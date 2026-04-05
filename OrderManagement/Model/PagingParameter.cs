namespace OrderManagement.Model
{
    public class PagingParameter
    {   
        public string? FilterText { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MaxCloudRate { get; set; }

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
