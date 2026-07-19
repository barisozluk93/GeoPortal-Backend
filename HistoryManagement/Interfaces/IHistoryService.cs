using HistoryManagement.Entities;
using HistoryManagement.Model;
namespace HistoryManagement.Interfaces;
public interface IHistoryService
{
    Task<Result<HistoryRecord>> Save(HistoryRecord history);
    Task<Result<PagingResult<PagedList<HistoryRecord>>>> Paginate(PagingParameter pagingParameter, string entityType, string recordId, string? serviceName, string? operationType);
}
