using SupportManagement.Entities;
using SupportManagement.Model;

namespace SupportManagement.Interfaces;

public interface IContactService
{
    Task<Result<Ticket>> CreateTicketFromContactAsync(ContactFormRequestModel request);
}
