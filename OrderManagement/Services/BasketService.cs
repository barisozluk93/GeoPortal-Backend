using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using OrderManagement.DbContexts;
using OrderManagement.Entity;
using OrderManagement.Enums;
using OrderManagement.Interfaces;
using OrderManagement.Model;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace OrderManagement.Services;

public class BasketService : IBasketService
{
    private readonly OrderManagementContext _dbContext;

    private const string ImageTypeMono = "Mono";
    private const string ImageTypeStereo = "Stereo";
    private const string DefaultItemType = "satelliteImage";
    private const string EmptyProcessingOptionsJson = "[]";

    public BasketService(
        OrderManagementContext dbContext,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
    }

    public Task<Result<List<Basket>>> GetBasketList(long userId, string token)
        => GetBasketItems(userId: userId, basketId: null, activeOnly: true);

    public Task<Result<List<Basket>>> GetOrderBasketList(long id, string token)
        => GetBasketItems(userId: null, basketId: id, activeOnly: false);

    public async Task<Result<Basket>> Save(Basket newBasket)
    {
        var result = new Result<Basket>();

        if (newBasket == null)
            return Fail(result, "Sepet bilgisi boş olamaz.");

        if (newBasket.UserId <= 0)
            return Fail(result, "Kullanıcı bilgisi zorunludur.");

        if (newBasket.ProductId <= 0 && newBasket.Product == null)
            return Fail(result, "Ürün bilgisi zorunludur.");

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var basket = await GetOrCreateActiveBasket(newBasket.UserId);
            var productId = await ResolveProductId(newBasket);

            var item = await FindSameItem(
                basket.Id,
                productId,
                newBasket.AoiId,
                newBasket.RequestWkt);

            if (item == null)
            {
                item = new BasketProduct
                {
                    BasketId = basket.Id,
                    ProductId = productId
                };

                await _dbContext.BasketProducts.AddAsync(item);
            }

            ApplyItem(item, newBasket);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            result.SetData(await ToResponse(item, basket));
            result.SetMessage("İşlem başarı ile gerçekleşti.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            result.SetIsSuccess(false);
            result.SetMessage(GetExceptionMessage(ex));
        }

        return result;
    }

    public async Task<Result<List<Basket>>> SaveAll(List<Basket> basketList)
    {
        var result = new Result<List<Basket>>();

        if (basketList == null || basketList.Count == 0)
            return Fail(result, "Sepet listesi boş olamaz.");

        var userId = basketList[0].UserId;

        if (userId <= 0 || basketList.Any(x => x.UserId != userId))
            return Fail(result, "Sepet kalemlerinin kullanıcı bilgisi geçersizdir.");

        if (basketList.Any(x => x.ProductId <= 0 && x.Product == null))
            return Fail(result, "Sepet kalemlerinde ürün bilgisi zorunludur.");

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var basket = await GetOrCreateActiveBasket(userId);
            var affectedItems = new List<BasketProduct>();

            foreach (var request in basketList)
            {
                var productId = await ResolveProductId(request);

                var item = await FindSameItem(
                    basket.Id,
                    productId,
                    request.AoiId,
                    request.RequestWkt);

                if (item == null)
                {
                    item = new BasketProduct
                    {
                        BasketId = basket.Id,
                        ProductId = productId
                    };

                    await _dbContext.BasketProducts.AddAsync(item);
                }

                ApplyItem(item, request);
                affectedItems.Add(item);
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            var responses = new List<Basket>(affectedItems.Count);

            foreach (var item in affectedItems)
                responses.Add(await ToResponse(item, basket));

            result.SetData(responses);
            result.SetMessage("İşlem başarı ile gerçekleşti.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            result.SetIsSuccess(false);
            result.SetMessage(GetExceptionMessage(ex));
        }

        return result;
    }

    public Task<Result<Basket>> Delete(long id, long productId)
        => DeleteInternal(id, productId, deleteAll: false);

    public Task<Result<Basket>> DeleteAll(long id, long productId)
        => DeleteInternal(id, productId, deleteAll: true);

    private async Task<Result<Basket>> DeleteInternal(
        long basketId,
        long productId,
        bool deleteAll)
    {
        var result = new Result<Basket>();

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            var basket = await _dbContext.Baskets.FirstOrDefaultAsync(x =>
                x.Id == basketId &&
                x.IsActive &&
                !x.IsDeleted);

            if (basket == null)
                return Fail(result, "Böyle bir sepet bulunmamaktadır.");

            var query = _dbContext.BasketProducts.Where(x =>
                x.BasketId == basketId &&
                x.ProductId == productId &&
                x.IsActive &&
                !x.IsDeleted);

            List<BasketProduct> items;

            if (deleteAll)
            {
                items = await query.ToListAsync();
            }
            else
            {
                var item = await query.OrderBy(x => x.Id).FirstOrDefaultAsync();
                items = item == null ? new List<BasketProduct>() : new List<BasketProduct> { item };
            }

            if (items.Count == 0)
                return Fail(result, "Sepette bu ürüne ait kayıt bulunmamaktadır.");

            foreach (var item in items)
            {
                item.IsActive = false;
                item.IsDeleted = true;
            }

            var deletedItemIds = items.Select(x => x.Id).ToList();

            var hasOtherActiveItem = await _dbContext.BasketProducts.AnyAsync(x =>
                x.BasketId == basketId &&
                x.IsActive &&
                !x.IsDeleted &&
                !deletedItemIds.Contains(x.Id));

            if (!hasOtherActiveItem)
            {
                basket.IsActive = false;
                basket.IsDeleted = true;
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            result.SetData(basket);
            result.SetMessage("İşlem başarı ile gerçekleşti.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            result.SetIsSuccess(false);
            result.SetMessage(GetExceptionMessage(ex));
        }

        return result;
    }

    private async Task<Result<List<Basket>>> GetBasketItems(
        long? userId,
        long? basketId,
        bool activeOnly)
    {
        var result = new Result<List<Basket>>();

        try
        {
            var basketQuery = _dbContext.Baskets
                .AsNoTracking()
                .AsQueryable();

            if (userId.HasValue)
            {
                basketQuery = basketQuery.Where(x =>
                    x.UserId == userId.Value &&
                    x.IsActive &&
                    !x.IsDeleted);
            }

            if (basketId.HasValue)
            {
                basketQuery = basketQuery.Where(x =>
                    x.Id == basketId.Value &&
                    !x.IsDeleted);
            }

            var basket = await basketQuery
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (basket == null)
            {
                result.SetData(new List<Basket>());
                result.SetMessage("İşlem başarı ile gerçekleşti.");
                return result;
            }

            var itemQuery = _dbContext.BasketProducts
                .AsNoTracking()
                .Include(x => x.Product)
                    .ThenInclude(x => x.Classes)
                .Where(x =>
                    x.BasketId == basket.Id &&
                    !x.IsDeleted);

            if (activeOnly)
                itemQuery = itemQuery.Where(x => x.IsActive);

            var items = await itemQuery
                .OrderBy(x => x.Id)
                .ToListAsync();

            var list = new List<Basket>(items.Count);

            foreach (var item in items)
                list.Add(await ToResponse(item, basket));

            result.SetData(list);
            result.SetMessage("İşlem başarı ile gerçekleşti.");
        }
        catch (Exception ex)
        {
            result.SetIsSuccess(false);
            result.SetMessage(GetExceptionMessage(ex));
        }

        return result;
    }

    private async Task<long> ResolveProductId(Basket request)
    {
        if (request.ProductId > 0)
            return request.ProductId;

        if (request.Product == null)
            throw new InvalidOperationException("Ürün bilgisi bulunamadı.");

        var product = PrepareCustomProductForSave(request.Product);

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        request.ProductId = product.Id;
        request.Product = product;

        return product.Id;
    }

    private async Task<BasketProduct?> FindSameItem(
        long basketId,
        long productId,
        string? aoiId,
        string? requestWkt)
    {
        var requestHash = CreateRequestHash(requestWkt);
        var normalizedAoiId = NormalizeNullableText(aoiId);

        var candidates = await _dbContext.BasketProducts
            .Where(x =>
                x.BasketId == basketId &&
                x.ProductId == productId &&
                x.AoiId == normalizedAoiId &&
                x.RequestHash == requestHash &&
                x.IsActive &&
                !x.IsDeleted)
            .ToListAsync();

        // SHA-256 çakışması pratikte beklenmez. Yine de gerçek WKT karşılaştırması
        // yapılarak yanlış kalemin eşleşmesi engellenir.
        var normalizedRequestWkt = NormalizeWkt(requestWkt);

        return candidates.FirstOrDefault(x =>
            NormalizeWkt(x.RequestWkt) == normalizedRequestWkt);
    }

    private static void ApplyItem(BasketProduct item, Basket request)
    {
        var count = Math.Max(1, Convert.ToInt32(request.NumberOf));
        var area = Math.Max(0d, Convert.ToDouble(request.RequestAreaKm2));

        var requestedUnitPrice = Math.Max(0d, Convert.ToDouble(request.UnitPrice));
        var productPrice = request.Product == null
            ? 0d
            : Math.Max(0d, Convert.ToDouble(request.Product.Price));

        var unitPrice = requestedUnitPrice > 0d
            ? requestedUnitPrice
            : productPrice;

        var requestedBaseTotal = Math.Max(0d, Convert.ToDouble(request.BaseTotalPrice));
        var baseTotal = requestedBaseTotal > 0d
            ? requestedBaseTotal
            : area * unitPrice;

        var processingJson = ResolveProcessingOptionsJson(request);
        var processingTotal = Math.Max(0d, Convert.ToDouble(request.ProcessingTotalPrice));

        var requestedCalculatedTotal = Math.Max(
            0d,
            Convert.ToDouble(request.CalculatedTotalPrice));

        var calculatedTotal = requestedCalculatedTotal > 0d
            ? requestedCalculatedTotal
            : (baseTotal + processingTotal) * count;

        item.NumberOf = count;

        item.AoiId = NormalizeNullableText(request.AoiId);
        item.AoiName = NormalizeNullableText(request.AoiName);
        item.AoiWkt = NormalizeNullableText(request.AoiWkt);

        item.RequestWkt = NormalizeNullableText(request.RequestWkt);
        item.RequestHash = CreateRequestHash(request.RequestWkt);
        item.IntersectionWkt = NormalizeNullableText(request.IntersectionWkt);

        item.RequestAreaKm2 = area;
        item.UnitPrice = unitPrice;
        item.BaseTotalPrice = baseTotal;

        item.ProcessingOptionsJson = processingJson;
        item.ProcessingTotalPrice = processingTotal;
        item.CalculatedTotalPrice = calculatedTotal;

        item.ItemType = string.IsNullOrWhiteSpace(request.ItemType)
            ? DefaultItemType
            : request.ItemType.Trim();

        item.IsActive = true;
        item.IsDeleted = false;
    }

    private async Task<Basket> ToResponse(BasketProduct item, Basket basket)
    {
        var product = item.Product ?? await _dbContext.Products
            .AsNoTracking()
            .Include(x => x.Classes)
            .FirstAsync(x => x.Id == item.ProductId);

        PrepareProductForResponse(product);

        object processingOptions;

        try
        {
            processingOptions = JsonConvert.DeserializeObject(
                string.IsNullOrWhiteSpace(item.ProcessingOptionsJson)
                    ? EmptyProcessingOptionsJson
                    : item.ProcessingOptionsJson) ?? Array.Empty<object>();
        }
        catch
        {
            processingOptions = Array.Empty<object>();
        }

        return new Basket
        {
            Id = basket.Id,
            BasketItemId = item.Id,
            UserId = basket.UserId,
            ProductId = item.ProductId,
            Product = product,
            IsActive = item.IsActive,
            IsDeleted = item.IsDeleted,
            NumberOf = item.NumberOf,
            AoiId = item.AoiId,
            AoiName = item.AoiName,
            AoiWkt = item.AoiWkt,
            RequestWkt = item.RequestWkt,
            IntersectionWkt = item.IntersectionWkt,
            RequestAreaKm2 = item.RequestAreaKm2,
            UnitPrice = item.UnitPrice,
            BaseTotalPrice = item.BaseTotalPrice,
            ProcessingOptionsJson = item.ProcessingOptionsJson,
            ProcessingOptions = processingOptions,
            ProcessingTotalPrice = item.ProcessingTotalPrice,
            CalculatedTotalPrice = item.CalculatedTotalPrice,
            ItemType = item.ItemType
        };
    }

    private async Task<Basket> GetOrCreateActiveBasket(long userId)
    {
        var basket = await _dbContext.Baskets.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.IsActive &&
            !x.IsDeleted);

        if (basket != null)
            return basket;

        basket = new Basket
        {
            UserId = userId,
            IsActive = true,
            IsDeleted = false
        };

        await _dbContext.Baskets.AddAsync(basket);
        await _dbContext.SaveChangesAsync();

        return basket;
    }

    private static string ResolveProcessingOptionsJson(Basket request)
    {
        if (request.ProcessingOptions != null)
            return JsonConvert.SerializeObject(request.ProcessingOptions);

        return string.IsNullOrWhiteSpace(request.ProcessingOptionsJson)
            ? EmptyProcessingOptionsJson
            : request.ProcessingOptionsJson;
    }

    private static Product PrepareCustomProductForSave(Product product)
    {
        if (!string.IsNullOrWhiteSpace(product.Wkt))
        {
            var geometry = new WKTReader().Read(product.Wkt);
            geometry.SRID = 4326;
            product.Geometry = geometry;
        }

        product.Id = 0;
        product.ImageType = NormalizeImageType(product.ImageType);
        product.IsDeleted = false;
        product.IsCustomArea = true;

        if (product.CategoryId == 0)
            product.CategoryId = (int)ProductCategory.CustomArea;

        product.Name = string.IsNullOrWhiteSpace(product.Name)
            ? "Özel Alan"
            : product.Name;

        product.Currency = string.IsNullOrWhiteSpace(product.Currency)
            ? "TRY"
            : product.Currency;

        product.PriceStr = string.IsNullOrWhiteSpace(product.PriceStr)
            ? $"₺{product.Price}"
            : product.PriceStr;

        product.Provider = string.IsNullOrWhiteSpace(product.Provider)
            ? "Custom Area"
            : product.Provider;

        product.SourceLabel = string.IsNullOrWhiteSpace(product.SourceLabel)
            ? "Custom Area"
            : product.SourceLabel;

        return product;
    }

    private static void PrepareProductForResponse(Product product)
    {
        product.ImageType = NormalizeImageType(product.ImageType);

        if (product.Geometry != null)
        {
            product.Wkt = new WKTWriter().Write(product.Geometry);
            product.Geometry = null;
        }
    }

    private static string NormalizeImageType(string? value)
        => value?.Equals(ImageTypeStereo, StringComparison.OrdinalIgnoreCase) == true
            ? ImageTypeStereo
            : ImageTypeMono;

    private static string? NormalizeNullableText(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string NormalizeWkt(string? requestWkt)
    {
        if (string.IsNullOrWhiteSpace(requestWkt))
            return string.Empty;

        var builder = new StringBuilder(requestWkt.Length);

        foreach (var character in requestWkt)
        {
            if (!char.IsWhiteSpace(character))
                builder.Append(char.ToUpperInvariant(character));
        }

        return builder.ToString();
    }

    private static string? CreateRequestHash(string? requestWkt)
    {
        var normalized = NormalizeWkt(requestWkt);

        if (string.IsNullOrEmpty(normalized))
            return null;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(normalized);
        var hash = sha256.ComputeHash(bytes);

        return Convert.ToHexString(hash);
    }

    private static string GetExceptionMessage(Exception exception)
        => exception.InnerException?.Message ?? exception.Message;

    private static TResult Fail<TResult>(TResult result, string message)
        where TResult : class
    {
        dynamic dynamicResult = result;
        dynamicResult.SetIsSuccess(false);
        dynamicResult.SetMessage(message);
        return result;
    }
}
