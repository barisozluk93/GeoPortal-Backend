using SupportManagement.Entities;
using SupportManagement.Model;

namespace SupportManagement.Interfaces;

public interface ITicketService
{
    Task<Result<PagingResult<PagedList<Ticket>>>> Paginate(PagingParameter pagingParameter, string? ticketNoFiler, string? customerFilter, string? emailFilter, string? subjectFilter, string? statusFilter, string? lastMesageDateFromFilter, string? lastMesageDateToFilter);
    Task<Result<Ticket?>> GetByIdAsync(long id);
    Task<Result<bool>> UpdateStatusAsync(long id, string status);
    Task<Result<bool>> ReplyAsync(long ticketId, string adminEmail, string message);
    Task<byte[]> ExportExcel(string token);
}
