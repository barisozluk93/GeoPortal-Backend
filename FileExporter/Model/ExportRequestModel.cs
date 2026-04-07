namespace FileExporter.Model
{
    public class ExportRequestModel
    {
        public string FileName { get; set; } = "export";
        public string SheetName { get; set; } = "Sheet1";
        public string Title { get; set; } = "Export";
        public List<ExportColumnModel> Columns { get; set; } = new();
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
    }
}
