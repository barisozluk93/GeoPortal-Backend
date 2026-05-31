namespace OrderManagement.Model
{
    public class ProductSmartFilterRequest
    {
        public string? ImageType { get; set; }
        public string? Provider { get; set; }

        public DateTime? AcquisitionStartDate { get; set; }
        public DateTime? AcquisitionEndDate { get; set; }

        public decimal? MaxCloudRate { get; set; }

        public decimal? MinCloudRate { get; set; }

        public decimal? MinOffNadir { get; set; }
        public decimal? MaxOffNadir { get; set; }

        public decimal? MinResolution { get; set; }
        public decimal? MaxResolution { get; set; }

        public string? SpectralResolution { get; set; }

        public string? Wkt { get; set; }
    }
}
