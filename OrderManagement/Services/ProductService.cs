using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using OrderManagement.DbContexts;
using OrderManagement.Entity;
using OrderManagement.Enums;
using OrderManagement.Interfaces;
using OrderManagement.Model;
using System.Data;

namespace OrderManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly OrderManagementContext _dbContext;
        private readonly IConfiguration _configuration;

        public ProductService(OrderManagementContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Result<PagingResult<PagedList<Product>>>> Paginate(PagingParameter pagingParameter)
        {
            var result = new Result<PagingResult<PagedList<Product>>>();

            string? lowerFilterText = string.IsNullOrWhiteSpace(pagingParameter.FilterText)
                ? null
                : pagingParameter.FilterText.ToLower();

            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted);

            try
            {
                var queryable = _dbContext.Products
                    .AsNoTracking()
                    .Where(x =>
                        (x.CategoryId == (int)ProductCategory.Market ||
                         x.CategoryId == (int)ProductCategory.CustomArea) &&
                        x.IsInMarket &&
                        (pagingParameter.MaxCloudRate <= -1 ||
                            (x.CloudRate.HasValue && x.CloudRate <= pagingParameter.MaxCloudRate)) &&
                        (pagingParameter.MaxPrice <= -1 ||
                            x.Price <= pagingParameter.MaxPrice) &&
                        (string.IsNullOrEmpty(lowerFilterText) ||
                            (x.Name ?? "").ToLower().Contains(lowerFilterText) ||
                            (x.ImageId ?? "").ToLower().Contains(lowerFilterText) ||
                            (x.City ?? "").ToLower().Contains(lowerFilterText) ||
                            (x.District ?? "").ToLower().Contains(lowerFilterText) ||
                            (x.Provider ?? "").ToLower().Contains(lowerFilterText) ||
                            (x.ImageType ?? "").ToLower().Contains(lowerFilterText) ||
                            (x.Satellite ?? "").ToLower().Contains(lowerFilterText) ||
                            (x.Sensor ?? "").ToLower().Contains(lowerFilterText) ||
                            (x.SensorMode ?? "").ToLower().Contains(lowerFilterText))
                    )
                    .Select(x => new Product
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ImageId = x.ImageId,

                        DownloadLink = x.DownloadLink,

                        Price = x.Price,
                        PriceStr = x.PriceStr,
                        Currency = x.Currency,

                        IsDeleted = x.IsDeleted,
                        IsInMarket = x.IsInMarket,
                        IsCustomArea = x.IsCustomArea,
                        CategoryId = x.CategoryId,

                        City = x.City,
                        District = x.District,
                        Provider = x.Provider,
                        SourceLabel = x.SourceLabel,

                        AcquisitionDate = x.AcquisitionDate,
                        AcquisitionStartDate = x.AcquisitionStartDate,
                        AcquisitionEndDate = x.AcquisitionEndDate,

                        Resolution = x.Resolution,
                        CloudRate = x.CloudRate,
                        AreaKm2 = x.AreaKm2,

                        SunElevation = x.SunElevation,
                        SunAzimuth = x.SunAzimuth,
                        OffNadirAngle = x.OffNadirAngle,

                        Satellite = x.Satellite,
                        Sensor = x.Sensor,
                        ProcessingLevel = x.ProcessingLevel,

                        ImageType = x.ImageType,
                        SensorMode = x.SensorMode,
                        ProductType = x.ProductType,
                        ProductLevel = x.ProductLevel,
                        BandId = x.BandId,
                        OutputFormat = x.OutputFormat,

                        ThumbnailUrl = x.ThumbnailUrl,
                        PreviewUrl = x.PreviewUrl,
                        MetadataUrl = x.MetadataUrl,

                        Description = x.Description,

                        IsOrthorectified = x.IsOrthorectified,
                        IsPansharpened = x.IsPansharpened,
                        IsClassified = x.IsClassified,

                        Classes = x.Classes
                    });

                var pagination = PagedList<Product>.ToPagedList(
                    queryable,
                    pagingParameter.PageNumber,
                    pagingParameter.PageSize);

                result.SetData(new PagingResult<PagedList<Product>>
                {
                    Items = pagination,
                    TotalCount = pagination.TotalCount
                });

                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<Product>> GetById(long id)
        {
            var result = new Result<Product>();

            try
            {
                var product = await _dbContext.Products
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x => new Product
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ImageId = x.ImageId,

                        FootprintPath = x.FootprintPath,
                        GeoTiffPath = x.GeoTiffPath,
                        PreviewPath = x.PreviewPath,
                        QuicklookPath = x.QuicklookPath,
                        MetadataPath = x.MetadataPath,
                        DownloadLink = x.DownloadLink,

                        Price = x.Price,
                        PriceStr = x.PriceStr,
                        Currency = x.Currency,

                        CategoryId = x.CategoryId,
                        IsDeleted = x.IsDeleted,
                        IsInMarket = x.IsInMarket,
                        IsCustomArea = x.IsCustomArea,

                        City = x.City,
                        District = x.District,
                        Provider = x.Provider,
                        SourceLabel = x.SourceLabel,

                        AcquisitionDate = x.AcquisitionDate,
                        AcquisitionStartDate = x.AcquisitionStartDate,
                        AcquisitionEndDate = x.AcquisitionEndDate,

                        Resolution = x.Resolution,
                        CloudRate = x.CloudRate,
                        AreaKm2 = x.AreaKm2,

                        SunElevation = x.SunElevation,
                        SunAzimuth = x.SunAzimuth,
                        OffNadirAngle = x.OffNadirAngle,

                        Satellite = x.Satellite,
                        Sensor = x.Sensor,
                        ProcessingLevel = x.ProcessingLevel,

                        OrderId = x.OrderId,
                        StripId = x.StripId,
                        CatalogId = x.CatalogId,
                        ImageDescriptor = x.ImageDescriptor,
                        ImageType = x.ImageType,
                        SensorMode = x.SensorMode,
                        BandId = x.BandId,
                        ProductType = x.ProductType,
                        ProductLevel = x.ProductLevel,
                        RadiometricLevel = x.RadiometricLevel,
                        OutputFormat = x.OutputFormat,
                        SpatialReference = x.SpatialReference,
                        ScanDirection = x.ScanDirection,
                        DataOwner = x.DataOwner,

                        ThumbnailUrl = x.ThumbnailUrl,
                        PreviewUrl = x.PreviewUrl,
                        MetadataUrl = x.MetadataUrl,

                        Description = x.Description,

                        IsOrthorectified = x.IsOrthorectified,
                        IsPansharpened = x.IsPansharpened,
                        IsClassified = x.IsClassified,

                        BboxMinX = x.BboxMinX,
                        BboxMinY = x.BboxMinY,
                        BboxMaxX = x.BboxMaxX,
                        BboxMaxY = x.BboxMaxY,

                        Wkt = x.Geometry != null ? x.Geometry.AsText() : null,

                        Classes = x.Classes != null
                            ? x.Classes.Select(c => new ProductClass
                            {
                                Id = c.Id,
                                ProductId = c.ProductId,
                                ClassName = c.ClassName,
                                PixelCount = c.PixelCount,
                                ColorHex = c.ColorHex
                            }).ToList()
                            : null
                    })
                    .FirstOrDefaultAsync();

                result.SetData(product);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<List<ProductSmartFilterResult>> SmartFilterAsync(ProductSmartFilterRequest request)
        {
            var query = _dbContext.Products
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsInMarket &&
                    x.CategoryId == (int)ProductCategory.Market);

            if (!string.IsNullOrWhiteSpace(request.Wkt))
            {
                var reader = new WKTReader();
                var searchGeometry = reader.Read(request.Wkt);
                searchGeometry.SRID = 4326;

                query = query.Where(x =>
                    x.Geometry != null &&
                    x.Geometry.Intersects(searchGeometry));
            }

            if (!string.IsNullOrWhiteSpace(request.ImageType))
            {
                var imageType = request.ImageType.Trim().ToLower();

                query = query.Where(x =>
                    x.ImageType != null &&
                    x.ImageType.ToLower() == imageType);
            }

            if (!string.IsNullOrWhiteSpace(request.Provider))
            {
                var provider = request.Provider.Trim().ToLower();

                query = query.Where(x =>
                    x.Provider != null &&
                    x.Provider.ToLower().Contains(provider));
            }

            if (request.AcquisitionStartDate.HasValue)
            {
                query = query.Where(x =>
                    x.AcquisitionDate.HasValue &&
                    x.AcquisitionDate.Value.Date >= request.AcquisitionStartDate.Value.Date);
            }

            if (request.AcquisitionEndDate.HasValue)
            {
                query = query.Where(x =>
                    x.AcquisitionDate.HasValue &&
                    x.AcquisitionDate.Value.Date <= request.AcquisitionEndDate.Value.Date);
            }

            if (request.MinCloudRate.HasValue)
            {
                query = query.Where(x =>
                    x.CloudRate.HasValue &&
                    x.CloudRate >= request.MinCloudRate.Value);
            }

            if (request.MaxCloudRate.HasValue)
            {
                query = query.Where(x =>
                    x.CloudRate.HasValue &&
                    x.CloudRate <= request.MaxCloudRate.Value);
            }

            if (request.MinOffNadir.HasValue)
            {
                query = query.Where(x =>
                    x.OffNadirAngle.HasValue &&
                    x.OffNadirAngle >= request.MinOffNadir.Value);
            }

            if (request.MaxOffNadir.HasValue)
            {
                query = query.Where(x =>
                    x.OffNadirAngle.HasValue &&
                    x.OffNadirAngle <= request.MaxOffNadir.Value);
            }

            if (request.MinResolution.HasValue)
            {
                query = query.Where(x =>
                    x.Resolution.HasValue &&
                    x.Resolution >= request.MinResolution.Value);
            }

            if (request.MaxResolution.HasValue)
            {
                query = query.Where(x =>
                    x.Resolution.HasValue &&
                    x.Resolution <= request.MaxResolution.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SpectralResolution))
            {
                var spectralResolution = request.SpectralResolution.Trim().ToLower();

                query = query.Where(x =>
                    (x.Sensor != null && x.Sensor.ToLower().Contains(spectralResolution)) ||
                    (x.SensorMode != null && x.SensorMode.ToLower().Contains(spectralResolution)) ||
                    (x.BandId != null && x.BandId.ToLower().Contains(spectralResolution)));
            }

            return await query
                .OrderByDescending(x => x.AcquisitionDate)
                .Select(x => new ProductSmartFilterResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageId = x.ImageId,

                    ImageType = x.ImageType,

                    SensorMode = x.SensorMode,
                    Provider = x.Provider,

                    AcquisitionDate = x.AcquisitionDate,
                    AcquisitionStartDate = x.AcquisitionStartDate,
                    AcquisitionEndDate = x.AcquisitionEndDate,

                    Resolution = x.Resolution,
                    SpectralResolution = x.BandId ?? x.SensorMode ?? x.Sensor,

                    CloudRate = x.CloudRate,
                    NadirAngle = x.OffNadirAngle,

                    BboxMinX = x.BboxMinX,
                    BboxMinY = x.BboxMinY,
                    BboxMaxX = x.BboxMaxX,
                    BboxMaxY = x.BboxMaxY,

                    PreviewUrl = x.PreviewUrl,
                    ThumbnailUrl = x.ThumbnailUrl,
                    MetadataUrl = x.MetadataUrl,

                    SunAzimuth = x.SunAzimuth,
                    SunElevation = x.SunElevation,

                    Price = x.Price,

                    Wkt = x.Geometry != null ? x.Geometry.AsText() : null
                })
                .ToListAsync();
        }
    }
}