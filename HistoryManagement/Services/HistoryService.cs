using System.Data;
using HistoryManagement.DbContexts;
using HistoryManagement.Entities;
using HistoryManagement.Interfaces;
using HistoryManagement.Model;
using Microsoft.EntityFrameworkCore;
namespace HistoryManagement.Services;
public sealed class HistoryService(HistoryManagementContext dbContext) : IHistoryService
{
    public async Task<Result<HistoryRecord>> Save(HistoryRecord history)
    {
        var result = new Result<HistoryRecord>();
        try
        {
            history.Id = 0;
            history.CreatedDate = DateTime.UtcNow;
            await dbContext.Histories.AddAsync(history);
            await dbContext.SaveChangesAsync();
            result.SetData(history);
            result.SetMessage("Tarihçe kaydı başarıyla oluşturuldu.");
        }
        catch (Exception ex) { result.SetIsSuccess(false); result.SetMessage(ex.Message); }
        return result;
    }

    public async Task<Result<PagingResult<PagedList<HistoryRecord>>>> Paginate(PagingParameter pagingParameter, string entityType, string recordId, string? serviceName, string? operationType)
    {
        var result = new Result<PagingResult<PagedList<HistoryRecord>>>();
        using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted);
        try
        {
            var query = dbContext.Histories.AsNoTracking().Where(x => x.EntityType == entityType && x.RecordId == recordId);
            if (!string.IsNullOrWhiteSpace(serviceName)) query = query.Where(x => x.ServiceName == serviceName);
            if (!string.IsNullOrWhiteSpace(operationType)) query = query.Where(x => x.OperationType == operationType);
            var pagination = PagedList<HistoryRecord>.ToPagedList(query.OrderByDescending(x => x.CreatedDate), pagingParameter.PageNumber, pagingParameter.PageSize);
            result.SetData(new PagingResult<PagedList<HistoryRecord>> { Items = pagination, TotalCount = pagination.TotalCount });
            result.SetMessage("İşlem başarı ile gerçekleşti.");
        }
        catch (Exception ex) { result.SetIsSuccess(false); result.SetMessage(ex.Message); }
        return result;
    }
}
