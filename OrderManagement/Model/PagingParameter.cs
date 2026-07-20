namespace OrderManagement.Model
{
    public class PagingParameter
    {
        // Sadece görüntü adı araması
        public string? FilterText { get; set; }

        // MONO / STEREO
        public string? ImageType { get; set; }

        // MSP1 / MSP2 / MSP3 / MSP4 / MSP5
        public string? Satellite { get; set; }

        public DateTime? AcquisitionStartDate { get; set; }
        public DateTime? AcquisitionEndDate { get; set; }

        public decimal? MinCloudRate { get; set; }
        public decimal? MaxCloudRate { get; set; }

        public decimal? MinOffNadir { get; set; }
        public decimal? MaxOffNadir { get; set; }

        public decimal? MinResolution { get; set; }
        public decimal? MaxResolution { get; set; }

        public string? SpectralResolution { get; set; }

        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
