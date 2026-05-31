using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OrderManagement.DbContexts;
using OrderManagement.Entity;
using OrderManagement.Enums;
using OrderManagement.Interfaces;
using OrderManagement.Model;
using System.Data;

namespace OrderManagement.Services
{
    public class BasketService : IBasketService
    {
        private readonly OrderManagementContext _dbContext;
        private readonly IConfiguration _configuration;

        private const string ImageTypeMono = "Mono";
        private const string ImageTypeStereo = "Stereo";

        public BasketService(
            OrderManagementContext dbContext,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Result<Basket>> DeleteAll(long id, long productId)
        {
            var result = new Result<Basket>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                var oldBasket = await _dbContext.Baskets
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted);

                if (oldBasket == null)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    return result;
                }

                var oldBasketProducts = await _dbContext.BasketProducts
                    .Where(x => x.BasketId == oldBasket.Id && x.IsActive && !x.IsDeleted)
                    .ToListAsync();

                var toBeDeletedList = oldBasketProducts
                    .Where(x => x.ProductId == productId)
                    .ToList();

                if (!toBeDeletedList.Any())
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Sepette bu ürüne ait kayıt bulunmamaktadır.");
                    return result;
                }

                toBeDeletedList.ForEach(x =>
                {
                    x.IsDeleted = true;
                    x.IsActive = false;
                });

                if (oldBasketProducts.Count == toBeDeletedList.Count)
                {
                    oldBasket.IsDeleted = true;
                    oldBasket.IsActive = false;
                }

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                result.SetData(oldBasket);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<Basket>> Delete(long id, long productId)
        {
            var result = new Result<Basket>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                var oldBasket = await _dbContext.Baskets
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted);

                if (oldBasket == null)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    return result;
                }

                var oldBasketProducts = await _dbContext.BasketProducts
                    .Where(x => x.BasketId == oldBasket.Id && x.IsActive && !x.IsDeleted)
                    .ToListAsync();

                var toBeDeleted = oldBasketProducts
                    .FirstOrDefault(x => x.ProductId == productId);

                if (toBeDeleted == null)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Sepette bu ürüne ait kayıt bulunmamaktadır.");
                    return result;
                }

                toBeDeleted.IsDeleted = true;
                toBeDeleted.IsActive = false;

                if (oldBasketProducts.Count == 1)
                {
                    oldBasket.IsDeleted = true;
                    oldBasket.IsActive = false;
                }

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                result.SetData(oldBasket);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<List<Basket>>> GetBasketList(long userId, string token)
        {
            var result = new Result<List<Basket>>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                var basket = await _dbContext.Baskets
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive && !x.IsDeleted);

                if (basket == null)
                {
                    result.SetData(null);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                    return result;
                }

                var productList = await _dbContext.BasketProducts
                    .Include(x => x.Product)
                        .ThenInclude(x => x.Classes)
                    .Where(x =>
                        x.BasketId == basket.Id &&
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Product != null)
                    .Select(x => x.Product)
                    .ToListAsync();

                var basketList = new List<Basket>();

                foreach (var product in productList)
                {
                    PrepareProductForResponse(product);

                    basketList.Add(new Basket
                    {
                        Id = basket.Id,
                        UserId = basket.UserId,
                        ProductId = product.Id,
                        IsDeleted = false,
                        IsActive = true,
                        Product = product
                    });
                }

                result.SetData(basketList);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<Basket>> Save(Basket newBasket)
        {
            var result = new Result<Basket>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                var basket = await GetOrCreateActiveBasket(newBasket.UserId);

                long productId = newBasket.ProductId;

                if (newBasket.ProductId == 0)
                {
                    if (newBasket.Product == null)
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Ürün bilgisi zorunludur.");
                        return result;
                    }

                    var newProduct = PrepareCustomProductForSave(newBasket.Product);
                    _dbContext.Products.Add(newProduct);

                    await _dbContext.SaveChangesAsync();

                    productId = newProduct.Id;
                    newBasket.ProductId = productId;
                    newBasket.Product = newProduct;
                }

                var alreadyExists = await _dbContext.BasketProducts.AnyAsync(x =>
                    x.BasketId == basket.Id &&
                    x.ProductId == productId &&
                    x.IsActive &&
                    !x.IsDeleted);

                if (!alreadyExists)
                {
                    _dbContext.BasketProducts.Add(new BasketProduct
                    {
                        ProductId = productId,
                        BasketId = basket.Id,
                        IsDeleted = false,
                        IsActive = true
                    });

                    await _dbContext.SaveChangesAsync();
                }

                transaction.Commit();

                if (newBasket.Product != null)
                    PrepareProductForResponse(newBasket.Product);

                result.SetData(newBasket);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<List<Basket>>> SaveAll(List<Basket> basketList)
        {
            var result = new Result<List<Basket>>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                if (basketList == null || !basketList.Any())
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Sepet listesi boş olamaz.");
                    return result;
                }

                var userId = basketList.First().UserId;
                var basket = await GetOrCreateActiveBasket(userId);

                foreach (var item in basketList)
                {
                    long productId = item.ProductId;

                    if (item.ProductId == 0)
                    {
                        if (item.Product == null)
                            continue;

                        var newProduct = PrepareCustomProductForSave(item.Product);
                        _dbContext.Products.Add(newProduct);

                        await _dbContext.SaveChangesAsync();

                        productId = newProduct.Id;
                        item.ProductId = productId;
                        item.Product = newProduct;
                    }

                    var alreadyExists = await _dbContext.BasketProducts.AnyAsync(x =>
                        x.BasketId == basket.Id &&
                        x.ProductId == productId &&
                        x.IsActive &&
                        !x.IsDeleted);

                    if (alreadyExists)
                        continue;

                    _dbContext.BasketProducts.Add(new BasketProduct
                    {
                        ProductId = productId,
                        BasketId = basket.Id,
                        IsDeleted = false,
                        IsActive = true
                    });

                    await _dbContext.SaveChangesAsync();
                }

                transaction.Commit();

                foreach (var item in basketList)
                {
                    if (item.Product != null)
                        PrepareProductForResponse(item.Product);
                }

                result.SetData(basketList);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<List<Basket>>> GetOrderBasketList(long id, string token)
        {
            var result = new Result<List<Basket>>();

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            try
            {
                var basket = await _dbContext.Baskets
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (basket == null)
                {
                    result.SetData(null);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                    return result;
                }

                var productList = await _dbContext.BasketProducts
                    .Include(x => x.Product)
                        .ThenInclude(x => x.Classes)
                    .Where(x =>
                        x.BasketId == basket.Id &&
                        !x.IsDeleted &&
                        x.Product != null)
                    .Select(x => x.Product)
                    .ToListAsync();

                var basketList = new List<Basket>();

                foreach (var product in productList)
                {
                    PrepareProductForResponse(product);

                    basketList.Add(new Basket
                    {
                        Id = basket.Id,
                        UserId = basket.UserId,
                        ProductId = product.Id,
                        IsDeleted = false,
                        IsActive = false,
                        Product = product
                    });
                }

                result.SetData(basketList);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        private async Task<Basket> GetOrCreateActiveBasket(long userId)
        {
            var basket = await _dbContext.Baskets
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive && !x.IsDeleted);

            if (basket != null)
                return basket;

            basket = new Basket
            {
                UserId = userId,
                IsDeleted = false,
                IsActive = true
            };

            _dbContext.Baskets.Add(basket);
            await _dbContext.SaveChangesAsync();

            return basket;
        }

        private static Product PrepareCustomProductForSave(Product product)
        {
            if (!string.IsNullOrWhiteSpace(product.Wkt))
            {
                var reader = new WKTReader();
                var geometry = reader.Read(product.Wkt);
                geometry.SRID = 4326;
                product.Geometry = geometry;
            }

            product.Id = 0;
            product.ImageType = NormalizeImageType(product.ImageType);
            product.IsDeleted = false;
            product.IsInMarket = product.IsInMarket;
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
                product.Wkt = ConvertToWkt(product.Geometry);
                product.Geometry = null;
            }
        }

        private static string NormalizeImageType(string? imageType)
        {
            if (string.IsNullOrWhiteSpace(imageType))
                return ImageTypeMono;

            if (imageType.Equals(ImageTypeStereo, StringComparison.OrdinalIgnoreCase))
                return ImageTypeStereo;

            if (imageType.Equals(ImageTypeMono, StringComparison.OrdinalIgnoreCase))
                return ImageTypeMono;

            return ImageTypeMono;
        }

        private static string ConvertToWkt(Geometry geometry)
        {
            var writer = new WKTWriter();
            return writer.Write(geometry);
        }
    }
}