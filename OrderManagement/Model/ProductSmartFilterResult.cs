namespace OrderManagement.Model
{
    public class ProductSmartFilterResult
    {
        public long Id { get; set; }
        public string? Name { get; set; }

        public string? ImageId { get; set; }
        public string? ImageType { get; set; } // Mono / Stereo

        public string? Provider { get; set; }

        public DateTime? AcquisitionDate { get; set; }
        public DateTime? AcquisitionStartDate { get; set; }
        public DateTime? AcquisitionEndDate { get; set; }

        public decimal? Resolution { get; set; }
        public string? SpectralResolution { get; set; }

        public decimal? CloudRate { get; set; }
        public decimal? NadirAngle { get; set; }

        public decimal? SunElevation { get; set; }
        public decimal? SunAzimuth { get; set; }

        public string? Satellite { get; set; }
        public string? Sensor { get; set; }
        public string? SensorMode { get; set; }

        public string? ProductType { get; set; }
        public string? ProductLevel { get; set; }
        public string? ProcessingLevel { get; set; }

        public string? BandId { get; set; }
        public string? OutputFormat { get; set; }

        public decimal? BboxMinX { get; set; }
        public decimal? BboxMinY { get; set; }
        public decimal? BboxMaxX { get; set; }
        public decimal? BboxMaxY { get; set; }

        public string? PreviewUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? MetadataUrl { get; set; }
        public string? Wkt { get; set; }

        public decimal? Price { get; set; }
    }
}