using UserManagement.Model;

namespace UserManagement.Interfaces
{
    public interface IExportGateway
    {
        Task<byte[]> ExportExcelAsync(ExportRequestModel request, string token);
    }
}
