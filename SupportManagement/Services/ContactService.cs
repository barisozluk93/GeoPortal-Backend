using Microsoft.EntityFrameworkCore;
using SupportManagement.DbContexts;
using SupportManagement.Entities;
using SupportManagement.Enums;
using SupportManagement.Interfaces;
using SupportManagement.Model;
using System.Data;

namespace SupportManagement.Services;

public class ContactService : IContactService
{
    private readonly SupportManagementDbContext _dbContext;

    public ContactService(SupportManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Ticket>> CreateTicketFromContactAsync(ContactFormRequestModel request)
    {
        var result = new Result<Ticket>();

        using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
        {
            try
            {
                var now = DateTime.UtcNow;

                var ticket = new Ticket
                {
                    TicketNo = GenerateTicketNo(),
                    CustomerName = request.FullName.Trim(),
                    CustomerEmail = request.BusinessEmail.Trim(),
                    Organization = string.IsNullOrWhiteSpace(request.Organization) ? null : request.Organization.Trim(),
                    Subject = request.Subject.Trim(),
                    Status = TicketStatus.New,
                    CreatedAtUtc = now,
                    LastMessageAtUtc = now,
                    IsClosed = false
                };
                _dbContext.Tickets.Add(ticket);


                var firstMessage = new TicketMessage
                {
                    TicketId = ticket.Id,
                    SenderType = SenderType.Customer,
                    Channel = MessageChannel.ContactForm,
                    SenderEmail = request.BusinessEmail.Trim(),
                    Body = request.Message.Trim(),
                    CreatedAtUtc = now
                };

                ticket.Messages.Add(firstMessage);
                await _dbContext.SaveChangesAsync();

                transaction.Commit();

                result.SetData(ticket);
                result.SetMessage("Ýþle baþarý ile gerçekleþtirildi.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                result.SetMessage(ex.Message);
                result.SetIsSuccess(false);
            }
        }

        return result;
    }

    private static string GenerateTicketNo()
    {
        return $"CT-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(100000, 999999)}";
    }
}
