using FileExporter.Model;

namespace FileExporter.Interfaces
{
    public interface IExportService
    {
        byte[] GenerateExcel(ExportRequestModel request);
            
    }
}