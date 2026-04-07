using OrderManagement.Model;

namespace OrderManagement.Interfaces
{
    public interface IExportGateway
    {
        Task<byte[]> ExportExcelAsync(ExportRequestModel request, string token);
    }
}
