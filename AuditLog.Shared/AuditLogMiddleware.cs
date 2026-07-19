using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuditLog.Shared;

public sealed class AuditLogMiddleware(
    RequestDelegate next,
    ILogger<AuditLogMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext context,
        IHttpClientFactory httpClientFactory,
        IOptions<AuditLogOptions> optionsAccessor)
    {
        var options = optionsAccessor.Value;

        if (!options.Enabled ||
            options.ExcludedPathPrefixes.Any(x =>
                context.Request.Path.StartsWithSegments(x)))
        {
            await next(context);
            return;
        }

        var correlationId =
            context.Request.Headers["X-Correlation-ID"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = context.TraceIdentifier;
        }

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        var stopwatch = Stopwatch.StartNew();
        var originalResponseBody = context.Response.Body;

        await using var temporaryResponseBody = new MemoryStream();
        context.Response.Body = temporaryResponseBody;

        Exception? error = null;
        string? responseBody = null;

        try
        {
            await next(context);

            stopwatch.Stop();

            temporaryResponseBody.Position = 0;

            if (ShouldReadResponseBody(context.Response))
            {
                responseBody = await ReadResponseBodyAsync(
                    temporaryResponseBody,
                    context.RequestAborted);
            }

            temporaryResponseBody.Position = 0;

            await temporaryResponseBody.CopyToAsync(
                originalResponseBody,
                context.RequestAborted);
        }
        catch (Exception ex)
        {
            error = ex;
            stopwatch.Stop();

            throw;
        }
        finally
        {
            context.Response.Body = originalResponseBody;

            var entry = CreateEntry(
                context,
                options,
                correlationId!,
                stopwatch.ElapsedMilliseconds,
                error,
                responseBody);

            _ = SendSafelyAsync(
                httpClientFactory,
                options,
                entry,
                logger);
        }
    }

    private static AuditLogEntry CreateEntry(
        HttpContext context,
        AuditLogOptions options,
        string correlationId,
        long durationMs,
        Exception? error,
        string? responseBody)
    {
        var user = context.User;

        var statusCode = error is null
            ? context.Response.StatusCode
            : StatusCodes.Status500InternalServerError;

        var isSuccess = ResolveIsSuccess(
            statusCode,
            responseBody,
            error);

        return new AuditLogEntry
        {
            TimestampUtc = DateTime.UtcNow,
            ServiceName = options.ServiceName,

            UserId =
                user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                user.FindFirstValue("sub") ??
                user.FindFirstValue("userId") ??
                user.FindFirstValue("id"),

            UserName =
                user.Identity?.Name ??
                user.FindFirstValue("username") ??
                user.FindFirstValue("name") ??
                user.FindFirstValue("unique_name") ??
                user.FindFirstValue("email"),

            Roles = string.Join(
                ",",
                user.FindAll(ClaimTypes.Role).Select(x => x.Value)),

            ActionType = ResolveActionType(
                context.Request.Method,
                context.Request.Path),

            HttpMethod = context.Request.Method,
            Path = context.Request.Path.Value ?? "/",
            QueryString = SanitizeQuery(
                context.Request.QueryString.Value),

            StatusCode = statusCode,
            IsSuccess = isSuccess,
            DurationMs = durationMs,

            IpAddress =
                context.Connection.RemoteIpAddress?.ToString(),

            UserAgent =
                context.Request.Headers.UserAgent.ToString(),

            CorrelationId = correlationId,

            ErrorMessage =
                error?.Message ??
                ResolveErrorMessage(responseBody, isSuccess)
        };
    }

    private static bool ResolveIsSuccess(
        int statusCode,
        string? responseBody,
        Exception? error)
    {
        if (error is not null)
        {
            return false;
        }

        if (statusCode < 200 || statusCode >= 400)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return true;
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);

            if (TryFindBooleanProperty(
                    document.RootElement,
                    "isSuccess",
                    out var isSuccess))
            {
                return isSuccess;
            }
        }
        catch (JsonException)
        {
            // JSON olmayan response için HTTP status code kullanılır.
        }

        return true;
    }

    private static string? ResolveErrorMessage(
        string? responseBody,
        bool isSuccess)
    {
        if (isSuccess || string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);

            if (TryFindStringProperty(
                    document.RootElement,
                    "message",
                    out var message))
            {
                return message;
            }

            if (TryFindStringProperty(
                    document.RootElement,
                    "errorMessage",
                    out var errorMessage))
            {
                return errorMessage;
            }
        }
        catch (JsonException)
        {
            // Response JSON değilse hata mesajı çıkarılmaz.
        }

        return null;
    }

    private static bool TryFindBooleanProperty(
        JsonElement element,
        string propertyName,
        out bool value)
    {
        value = false;

        if (element.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (!string.Equals(
                    property.Name,
                    propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            switch (property.Value.ValueKind)
            {
                case JsonValueKind.True:
                    value = true;
                    return true;

                case JsonValueKind.False:
                    value = false;
                    return true;

                case JsonValueKind.String:
                    return bool.TryParse(
                        property.Value.GetString(),
                        out value);

                case JsonValueKind.Number:
                    if (property.Value.TryGetInt32(out var number))
                    {
                        value = number != 0;
                        return true;
                    }

                    break;
            }
        }

        return false;
    }

    private static bool TryFindStringProperty(
        JsonElement element,
        string propertyName,
        out string? value)
    {
        value = null;

        if (element.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (!string.Equals(
                    property.Name,
                    propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (property.Value.ValueKind == JsonValueKind.String)
            {
                value = property.Value.GetString();
                return !string.IsNullOrWhiteSpace(value);
            }
        }

        return false;
    }

    private static bool ShouldReadResponseBody(
        HttpResponse response)
    {
        if (response.Body.CanSeek == false)
        {
            return false;
        }

        var contentType = response.ContentType;

        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        return contentType.Contains(
                   "application/json",
                   StringComparison.OrdinalIgnoreCase)
               ||
               contentType.Contains(
                   "application/problem+json",
                   StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<string?> ReadResponseBodyAsync(
        Stream responseBodyStream,
        CancellationToken cancellationToken)
    {
        if (!responseBodyStream.CanSeek)
        {
            return null;
        }

        responseBodyStream.Position = 0;

        using var reader = new StreamReader(
            responseBodyStream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync(cancellationToken);

        responseBodyStream.Position = 0;

        return body;
    }

    private static string ResolveActionType(
        string method,
        PathString path)
    {
        var value =
            path.Value?.ToLowerInvariant() ?? string.Empty;

        if (value.Contains("login"))
        {
            return "Login";
        }

        if (value.Contains("logout"))
        {
            return "Logout";
        }

        if (value.Contains("download") ||
            value.Contains("export"))
        {
            return "ExportDownload";
        }

        if (value.Contains("search") ||
            value.Contains("filter") ||
            value.Contains("paginate") ||
            value.Contains("getall"))
        {
            return "Search";
        }

        return method.ToUpperInvariant() switch
        {
            "GET" => "View",
            "POST" => "CreateOrExecute",
            "PUT" or "PATCH" => "Update",
            "DELETE" => "Delete",
            _ => "Other"
        };
    }

    private static string? SanitizeQuery(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return null;
        }

        return query.Length <= 2000
            ? query
            : query[..2000];
    }

    private static async Task SendSafelyAsync(
        IHttpClientFactory factory,
        AuditLogOptions options,
        AuditLogEntry entry,
        ILogger logger)
    {
        try
        {
            var client =
                factory.CreateClient("AuditLogClient");

            client.BaseAddress ??=
                new Uri(options.BaseUrl);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                options.IngestionPath)
            {
                Content = JsonContent.Create(entry)
            };

            request.Headers.TryAddWithoutValidation(
                "X-Audit-Api-Key",
                options.InternalApiKey);

            using var cts =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(3));

            using var response =
                await client.SendAsync(request, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Audit log gönderilemedi. StatusCode: {StatusCode}",
                    response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Audit log servisine ulaşılamadı; ana işlem etkilenmedi.");
        }
    }
}