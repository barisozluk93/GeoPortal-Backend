using MailKit.Search;
using OrderManagement.Entity;
using OrderManagement.Model;

namespace OrderManagement.Interfaces
{
    public interface IOrderService
    {
        Task<Result<PagingResult<PagedList<Order>>>> Paginate(PagingParameter pagingParameter, long userId, string? orderNoFiler, double? priceMinFilter, double? priceMaxFilter, string? orderDateFromFilter, string? orderDateToFilter, long? orderStatusStrFilter);
        Task<Result<PagingResult<PagedList<Order>>>> ComingPaginate(PagingParameter pagingParameter, string? orderNoFiler, double? priceMinFilter, double? priceMaxFilter, string? orderDateFromFilter, string? orderDateToFilter, long? orderStatusStrFilter);
        Task<Result<List<Order>>> GetOrderList(long userId);
        Task<Result<Order>> Save(Order product, string token);
        Task<Result<OrderProduct>> UpdateStatus(long id, long status, string token);
        Task<Result<Order>> GetById(long id, string token);
        Task<Result<OrderProduct>> GetComingOrderById(long id, string token);
        Task<Result<Order>> AddInvoice(long id, long fileId, string token);
        Task<Result<Order>> DeleteInvoice(long id, string token);
        Task<Result<ValidateApiKeyResponse>> ValidateApiKey(ValidateApiKeyRequest request);
        Task<byte[]> ExportExcel(string token);


    }
}
