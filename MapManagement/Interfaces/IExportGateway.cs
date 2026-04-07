using MapManagement.Model;

namespace MapManagement.Interfaces
{
    public interface IExportGateway
    {
        Task<byte[]> ExportExcelAsync(ExportRequestModel request, string token);
        Task<byte[]> ExportPdfAsync(ExportRequestModel request, string token);
    }
}
