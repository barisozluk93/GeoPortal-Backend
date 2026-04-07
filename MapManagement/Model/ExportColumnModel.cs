namespace MapManagement.Model
{
    public class ExportColumnModel
    {
        public string Key { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public int? Width { get; set; }
        public string? Format { get; set; }

        public string? DataType { get; set; }
    }
}
