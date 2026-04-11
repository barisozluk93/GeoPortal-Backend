using NetTopologySuite.Geometries;
using OrderManagement.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Entity
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? DownloadLink { get; set; }
        public decimal Price { get; set; }
        public string? PriceStr { get; set; }
        public bool IsDeleted { get; set; }
        public int CategoryId { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public string? Provider { get; set; }
        public decimal? Resolution { get; set; }
        public int? CloudRate { get; set; }
        public decimal? AreaKm2 { get; set; }
        public string? Currency { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewUrl { get; set; }
        public string? Description { get; set; }
        public bool? IsOrthorectified { get; set; }
        public bool? IsPansharpened { get; set; }
        public bool? IsClassified { get; set; }

        public Geometry? Geometry { get; set; }

        public decimal? BboxMinX { get; set; }
        public decimal? BboxMinY { get; set; }
        public decimal? BboxMaxX { get; set; }
        public decimal? BboxMaxY { get; set; }

        public ProductSourceType? SourceType { get; set; }
        public string? SourceLabel { get; set; }
        public string? RequestHash { get; set; }
        public bool IsCustomArea { get; set; }
        public bool IsInMarket { get; set; }

        public ICollection<ProductClass>? Classes { get; set; }

        [NotMapped]
        public string? Wkt { get; set; }
    }

    public class ProductClass
    {
        public long? Id { get; set; }
        public long? ProductId { get; set; }
        public string? ClassName { get; set; }
        public int? PixelCount { get; set; }
        public string? ColorHex { get; set; }

        public Product? Product { get; set; }
    }
}