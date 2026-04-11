using SupportManagement.Model;

namespace SupportManagement.Interfaces
{
    public interface IExportGateway
    {
        Task<byte[]> ExportExcelAsync(ExportRequestModel request, string token);
    }
}
