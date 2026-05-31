using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Entity
{
    public class Product
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;
        public string? ImageId { get; set; }

        // 📁 DOSYALAR
        public string? FootprintPath { get; set; }
        public string? GeoTiffPath { get; set; }
        public string? PreviewPath { get; set; }
        public string? QuicklookPath { get; set; }
        public string? MetadataPath { get; set; }

        public string? DownloadLink { get; set; }

        // 🌍 GEO
        public Geometry? Geometry { get; set; }

        public decimal? BboxMinX { get; set; }
        public decimal? BboxMinY { get; set; }
        public decimal? BboxMaxX { get; set; }
        public decimal? BboxMaxY { get; set; }

        public decimal? AreaKm2 { get; set; }

        public string? City { get; set; }
        public string? District { get; set; }

        // 📊 TEMEL METADATA
        public DateTime? AcquisitionDate { get; set; }
        public DateTime? AcquisitionStartDate { get; set; }
        public DateTime? AcquisitionEndDate { get; set; }

        public decimal? Resolution { get; set; }
        public decimal? CloudRate { get; set; }

        public decimal? SunElevation { get; set; }
        public decimal? SunAzimuth { get; set; }
        public decimal? OffNadirAngle { get; set; }

        public string? Satellite { get; set; }
        public string? Sensor { get; set; }
        public string? ProcessingLevel { get; set; }

        public string? SourceLabel { get; set; }
        public string? Provider { get; set; }

        // 🛰️ UYDU / ÜRÜN DETAYLARI
        public string? OrderId { get; set; }
        public string? StripId { get; set; }
        public string? CatalogId { get; set; }
        public string? ImageDescriptor { get; set; }
        public string? ImageType { get; set; }
        public string? SensorMode { get; set; }
        public string? BandId { get; set; }
        public string? ProductType { get; set; }
        public string? ProductLevel { get; set; }
        public string? RadiometricLevel { get; set; }
        public string? OutputFormat { get; set; }
        public string? SpatialReference { get; set; }
        public string? ScanDirection { get; set; }
        public string? DataOwner { get; set; }

        public bool? IsOrthorectified { get; set; }
        public bool? IsPansharpened { get; set; }
        public bool? IsClassified { get; set; }

        public string? ThumbnailUrl { get; set; }
        public string? PreviewUrl { get; set; }

        public string? MetadataUrl { get; set; }

        // 💰
        public decimal Price { get; set; }
        public string? PriceStr { get; set; }
        public string? Currency { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsInMarket { get; set; }
        public bool IsCustomArea { get; set; }

        public int CategoryId { get; set; }

        public string? Description { get; set; }

        // 🔗 RELATION
        public ICollection<ProductClass>? Classes { get; set; }

        [NotMapped]
        public string? Wkt { get; set; }
    }

    public class ProductClass
    {
        public long Id { get; set; }
        public long ProductId { get; set; }

        public string? ClassName { get; set; }
        public int? PixelCount { get; set; }
        public string? ColorHex { get; set; }

        public Product? Product { get; set; }
    }
}
