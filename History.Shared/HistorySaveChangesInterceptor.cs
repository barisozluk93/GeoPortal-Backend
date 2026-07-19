using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace History.Shared;

public sealed class HistorySaveChangesInterceptor(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    IOptions<HistoryOptions> optionsAccessor,
    ILogger<HistorySaveChangesInterceptor> logger)
    : SaveChangesInterceptor
{
    private readonly ConcurrentDictionary<Guid, List<PendingHistory>> _pending = new();

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        Capture(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Capture(eventData.Context);

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        PublishAsync(
                eventData.Context,
                CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await PublishAsync(
            eventData.Context,
            cancellationToken);

        return await base.SavedChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    public override void SaveChangesFailed(
        DbContextErrorEventData eventData)
    {
        Clear(eventData.Context);

        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        Clear(eventData.Context);

        return base.SaveChangesFailedAsync(
            eventData,
            cancellationToken);
    }

    private void Capture(DbContext? dbContext)
    {
        var options = optionsAccessor.Value;

        if (!options.Enabled ||
            dbContext is null ||
            dbContext.GetType().Name.Contains(
                "History",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            var items = dbContext.ChangeTracker
                .Entries()
                .Where(entry =>
                    entry.State is
                        EntityState.Added or
                        EntityState.Modified or
                        EntityState.Deleted)
                .Where(entry => !entry.Metadata.IsOwned())
                .Select(CreatePending)
                .Where(history => history is not null)
                .Cast<PendingHistory>()
                .ToList();

            if (items.Count == 0)
            {
                _pending.TryRemove(
                    dbContext.ContextId.InstanceId,
                    out _);

                return;
            }

            _pending[dbContext.ContextId.InstanceId] = items;
        }
        catch (Exception ex)
        {
            /*
             * History hazırlanırken oluşan bir hata,
             * ana SaveChanges işlemini bozmamalıdır.
             */
            logger.LogError(
                ex,
                "History değişiklikleri yakalanırken hata oluştu. " +
                "Ana veritabanı işlemi devam edecek.");

            _pending.TryRemove(
                dbContext.ContextId.InstanceId,
                out _);
        }
    }

    private PendingHistory? CreatePending(EntityEntry entry)
    {
        var primaryKey = entry.Metadata.FindPrimaryKey();

        if (primaryKey is null)
        {
            return null;
        }

        var changedProperties =
            new Dictionary<string, HistoryPropertyChange>();

        foreach (var property in entry.Properties)
        {
            /*
             * Primary key değerleri ChangesJson içine alınmıyor.
             * RecordId oluşturulurken ayrıca kullanılacak.
             */
            if (property.Metadata.IsPrimaryKey())
            {
                continue;
            }

            /*
             * Update işleminde yalnızca gerçekten değişmiş
             * alanları history kaydına al.
             */
            if (entry.State == EntityState.Modified &&
                !property.IsModified)
            {
                continue;
            }

            var oldValue = entry.State == EntityState.Added
                ? null
                : NormalizeHistoryValue(
                    property.OriginalValue);

            var newValue = entry.State == EntityState.Deleted
                ? null
                : NormalizeHistoryValue(
                    property.CurrentValue);

            changedProperties[property.Metadata.Name] =
                new HistoryPropertyChange
                {
                    OldValue = oldValue,
                    NewValue = newValue
                };
        }

        var operationType = DetectOperation(entry);

        string changesJson;

        try
        {
            changesJson = JsonSerializer.Serialize(
                changedProperties);
        }
        catch (Exception ex)
        {
            /*
             * Tek bir property serialize edilemiyorsa
             * SaveChanges işlemini engelleme.
             */
            logger.LogError(
                ex,
                "{EntityType} entity değişiklikleri JSON'a " +
                "çevrilirken hata oluştu.",
                entry.Metadata.ClrType.Name);

            changesJson = "{}";
        }

        return new PendingHistory(
            Entry: entry,
            KeyNames: primaryKey.Properties
                .Select(property => property.Name)
                .ToArray(),
            OperationType: operationType,
            ChangesJson: changesJson);
    }

    private static string DetectOperation(
        EntityEntry entry)
    {
        if (entry.State == EntityState.Added)
        {
            return "Create";
        }

        if (entry.State == EntityState.Deleted)
        {
            return "Delete";
        }

        var isDeletedProperty = entry.Properties
            .FirstOrDefault(property =>
                property.Metadata.Name.Equals(
                    "IsDeleted",
                    StringComparison.OrdinalIgnoreCase));

        if (isDeletedProperty is not null &&
            isDeletedProperty.IsModified &&
            isDeletedProperty.CurrentValue is true)
        {
            return "Delete";
        }

        var statusChanged = entry.Properties.Any(property =>
            property.IsModified &&
            IsStatusProperty(property.Metadata.Name));

        return statusChanged
            ? "StatusUpdate"
            : "Update";
    }

    private static bool IsStatusProperty(
        string propertyName)
    {
        return propertyName.Equals(
                   "Status",
                   StringComparison.OrdinalIgnoreCase) ||
               propertyName.Equals(
                   "StatusId",
                   StringComparison.OrdinalIgnoreCase) ||
               propertyName.EndsWith(
                   "Status",
                   StringComparison.OrdinalIgnoreCase) ||
               propertyName.EndsWith(
                   "StatusId",
                   StringComparison.OrdinalIgnoreCase);
    }

    private async Task PublishAsync(
        DbContext? dbContext,
        CancellationToken cancellationToken)
    {
        if (dbContext is null)
        {
            return;
        }

        if (!_pending.TryRemove(
                dbContext.ContextId.InstanceId,
                out var pendingItems))
        {
            return;
        }

        var options = optionsAccessor.Value;

        if (!options.Enabled)
        {
            return;
        }

        var user = httpContextAccessor
            .HttpContext?
            .User;

        var userId =
            user?.FindFirstValue(
                ClaimTypes.NameIdentifier) ??
            user?.FindFirstValue("id") ??
            user?.FindFirstValue("sub");

        var userName =
            user?.Identity?.Name ??
            user?.FindFirstValue("username") ??
            user?.FindFirstValue("unique_name") ??
            user?.FindFirstValue(ClaimTypes.Name);

        foreach (var item in pendingItems)
        {
            try
            {
                var recordId = GetRecordId(item);

                /*
                 * IDENTITY veya sequence ile üretilen kayıtlarda
                 * gerçek ID ancak SaveChanges sonrasında oluşur.
                 * Bu nedenle RecordId burada hesaplanıyor.
                 */
                if (string.IsNullOrWhiteSpace(recordId))
                {
                    logger.LogWarning(
                        "{EntityType} history kaydı için " +
                        "RecordId oluşturulamadı.",
                        item.Entry.Metadata.ClrType.Name);

                    continue;
                }

                var historyEntry = new HistoryEntry
                {
                    RecordId = recordId,
                    EntityType =
                        item.Entry.Metadata.ClrType.Name,

                    OperationType =
                        item.OperationType,

                    Description =
                        $"{item.Entry.Metadata.ClrType.Name} " +
                        $"{item.OperationType}",

                    ServiceName =
                        options.ServiceName,

                    UserId =
                        userId,

                    UserName =
                        userName,

                    CreatedDate =
                        DateTime.UtcNow,

                    ChangesJson =
                        item.ChangesJson
                };

                var client =
                    httpClientFactory.CreateClient(
                        "HistoryManagement");

                client.BaseAddress =
                    new Uri(options.BaseUrl);

                var requestJson =
                    JsonSerializer.Serialize(historyEntry);

                using var request =
                    new HttpRequestMessage(
                        HttpMethod.Post,
                        options.IngestionPath)
                    {
                        Content = new StringContent(
                            requestJson,
                            Encoding.UTF8,
                            "application/json")
                    };

                if (!string.IsNullOrWhiteSpace(
                        options.InternalApiKey))
                {
                    request.Headers.TryAddWithoutValidation(
                        "X-Internal-Api-Key",
                        options.InternalApiKey);
                }

                using var response =
                    await client.SendAsync(
                        request,
                        cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent =
                        await response.Content
                            .ReadAsStringAsync(
                                cancellationToken);

                    logger.LogWarning(
                        "History kaydı gönderilemedi. " +
                        "EntityType: {EntityType}, " +
                        "RecordId: {RecordId}, " +
                        "StatusCode: {StatusCode}, " +
                        "Response: {Response}",
                        historyEntry.EntityType,
                        historyEntry.RecordId,
                        response.StatusCode,
                        responseContent);
                }
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(
                    "History kaydı gönderme işlemi iptal edildi.");

                break;
            }
            catch (Exception ex)
            {
                /*
                 * History servisi çalışmıyor olsa bile
                 * ana veri kaydetme işlemi başarısız olmamalı.
                 */
                logger.LogError(
                    ex,
                    "History kaydı gönderilirken hata oluştu. " +
                    "EntityType: {EntityType}",
                    item.Entry.Metadata.ClrType.Name);
            }
        }
    }

    private static string GetRecordId(
        PendingHistory pendingHistory)
    {
        var keyValues = new List<string>();

        foreach (var keyName in pendingHistory.KeyNames)
        {
            var property =
                pendingHistory.Entry.Property(keyName);

            var value =
                property.CurrentValue ??
                property.OriginalValue;

            if (value is null)
            {
                continue;
            }

            keyValues.Add(
                Convert.ToString(
                    value,
                    System.Globalization.CultureInfo.InvariantCulture)
                ?? string.Empty);
        }

        return string.Join(",", keyValues);
    }

    private void Clear(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        _pending.TryRemove(
            dbContext.ContextId.InstanceId,
            out _);
    }

    private static object? NormalizeHistoryValue(
        object? value)
    {
        if (value is null)
        {
            return null;
        }

        /*
         * System.Text.Json varsayılan olarak NaN,
         * PositiveInfinity ve NegativeInfinity değerlerini
         * geçerli JSON sayısı olarak yazamaz.
         *
         * Bu değerleri metne çevirerek hem bilgiyi koruyoruz
         * hem de standart JSON oluşturuyoruz.
         */
        if (value is double doubleValue)
        {
            if (double.IsNaN(doubleValue))
            {
                return "NaN";
            }

            if (double.IsPositiveInfinity(doubleValue))
            {
                return "PositiveInfinity";
            }

            if (double.IsNegativeInfinity(doubleValue))
            {
                return "NegativeInfinity";
            }

            return doubleValue;
        }

        if (value is float floatValue)
        {
            if (float.IsNaN(floatValue))
            {
                return "NaN";
            }

            if (float.IsPositiveInfinity(floatValue))
            {
                return "PositiveInfinity";
            }

            if (float.IsNegativeInfinity(floatValue))
            {
                return "NegativeInfinity";
            }

            return floatValue;
        }

        /*
         * byte[] gibi alanların tamamını history'ye yazmak
         * veritabanını ve HTTP isteğini gereksiz büyütebilir.
         */
        if (value is byte[] byteArray)
        {
            return $"[Binary data: {byteArray.Length} bytes]";
        }

        /*
         * DateTime değerlerini UTC ve standart formatta sakla.
         */
        if (value is DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Utc =>
                    dateTime.ToString("O"),

                DateTimeKind.Local =>
                    dateTime.ToUniversalTime().ToString("O"),

                _ =>
                    DateTime.SpecifyKind(
                            dateTime,
                            DateTimeKind.Utc)
                        .ToString("O")
            };
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset
                .ToUniversalTime()
                .ToString("O");
        }

        if (value is Guid guid)
        {
            return guid.ToString();
        }

        if (value is Enum enumValue)
        {
            return enumValue.ToString();
        }

        return value;
    }

    private sealed record PendingHistory(
        EntityEntry Entry,
        string[] KeyNames,
        string OperationType,
        string ChangesJson);

    private sealed class HistoryPropertyChange
    {
        public object? OldValue { get; init; }

        public object? NewValue { get; init; }
    }
}