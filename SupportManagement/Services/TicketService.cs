using Microsoft.EntityFrameworkCore;
using SupportManagement.DbContexts;
using SupportManagement.Entities;
using SupportManagement.Enums;
using SupportManagement.Interfaces;
using SupportManagement.Model;
using System.Data;
using System.Globalization;

namespace SupportManagement.Services;

public class TicketService : ITicketService
{
    private readonly SupportManagementDbContext _dbContext;
    private readonly IEmailSender _emailSender;

    public TicketService(SupportManagementDbContext dbContext, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _emailSender = emailSender;
    }

        public async Task<Result<PagingResult<PagedList<Ticket>>>> Paginate(PagingParameter pagingParameter, string? ticketNoFiler, string? customerFilter, string? emailFilter, string? subjectFilter, string? statusFilter, string? lastMesageDateFromFilter, string? lastMesageDateToFilter)
        {
            var result = new Result<PagingResult<PagedList<Ticket>>>();
            
            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {

                    DateTime? lastMessageDateFrom = null;
                    DateTime? lastMessageDateTo = null;

                    if (!string.IsNullOrWhiteSpace(lastMesageDateFromFilter) &&
                        DateTime.TryParse(lastMesageDateFromFilter, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedFrom))
                    {
                        // datetime-local timezone göndermez, bunu local kabul edip UTC'ye çeviriyoruz
                        lastMessageDateFrom = DateTime.SpecifyKind(parsedFrom, DateTimeKind.Local).ToUniversalTime();
                    }

                    if (!string.IsNullOrWhiteSpace(lastMesageDateToFilter) &&
                        DateTime.TryParse(lastMesageDateToFilter, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTo))
                    {
                        // datetime-local timezone göndermez, bunu local kabul edip UTC'ye çeviriyoruz
                        lastMessageDateTo = DateTime.SpecifyKind(parsedTo, DateTimeKind.Local).ToUniversalTime();
                    }

                    var queryable = _dbContext.Tickets
                        .Include(x => x.Messages)
                        .Where(x =>
                            (!string.IsNullOrEmpty(ticketNoFiler) ? x.TicketNo.ToLower().Contains(ticketNoFiler.ToLower()) : true) &&
                            (!string.IsNullOrEmpty(customerFilter) ? x.CustomerName.ToLower().Contains(customerFilter.ToLower()) : true) &&
                            (!string.IsNullOrEmpty(emailFilter) ? x.CustomerEmail.ToLower().Contains(emailFilter.ToLower()) : true) &&
                            (!string.IsNullOrEmpty(subjectFilter) ? x.Subject.ToLower().Contains(subjectFilter.ToLower()) : true) &&
                            (!string.IsNullOrEmpty(statusFilter) ? x.Status.ToLower().Contains(statusFilter.ToLower()) : true) &&
                            (lastMessageDateFrom.HasValue ? x.LastMessageAtUtc >= lastMessageDateFrom.Value : true) &&
                            (lastMessageDateTo.HasValue ? x.LastMessageAtUtc <= lastMessageDateTo.Value : true)
                        );

                    var pagination = PagedList<Ticket>.ToPagedList(
                        queryable,
                        pagingParameter.PageNumber,
                        pagingParameter.PageSize
                    );

                    result.SetData(new PagingResult<PagedList<Ticket>>()
                    {
                        Items = pagination,
                        TotalCount = pagination.TotalCount,
                    });

                    result.SetMessage("Ýþlem baþarý ile gerçekleþti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }


    public async Task<Result<Ticket>> GetByIdAsync(long id)
    {
        var result = new Result<Ticket>();

        using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
        {
            try
            {
                var ticket = await _dbContext.Tickets
                    .Include(x => x.Messages)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (ticket is not null)
                {
                    ticket.Messages = ticket.Messages.OrderBy(m => m.CreatedAtUtc).ToList();
                }

                result.SetData(ticket);
                result.SetMessage("Ýþlem baþarý ile gerçekleþtirildi.");

            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
        }

        return result;
    }

    public async Task<Result<bool>> UpdateStatusAsync(long id, string status)
    {
        var result = new Result<bool>();

        using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
        {
            try
            {
                var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);
                if (ticket is null)
                {
                    result.SetData(false);
                    result.SetMessage("Böyle bir kayýt bulunamadý.");

                    return result;
                }

                ticket.Status = status;
                ticket.IsClosed = status == TicketStatus.Closed;

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                result.SetData(true);
                result.SetMessage("Ýþlem baþarý ile gerçekleþtirildi.");

            }
            catch (Exception ex)
            {
                transaction.Rollback();

                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
        }

        return result;        
    }

    public async Task<Result<bool>> ReplyAsync(long ticketId, string adminEmail, string message)
    {
        var result = new Result<bool>();

        using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
        {
            try
            {
                var ticket = await _dbContext.Tickets
                    .Include(x => x.Messages)
                    .FirstOrDefaultAsync(x => x.Id == ticketId);

                if (ticket is null)
                {
                    result.SetData(false);
                    result.SetMessage("Böyle bir kayýt bulunamadý.");
                    return result;
                }

                var lastExternalMessageId = ticket.Messages
                    .Where(x => !string.IsNullOrWhiteSpace(x.ExternalMessageId))
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .Select(x => x.ExternalMessageId)
                    .FirstOrDefault();

                var now = DateTime.UtcNow;
                var adminMessage = new TicketMessage
                {
                    TicketId = ticket.Id,
                    SenderType = SenderType.Admin,
                    Channel = MessageChannel.AdminPanel,
                    SenderEmail = adminEmail,
                    Body = message.Trim(),
                    InReplyToExternalMessageId = lastExternalMessageId,
                    CreatedAtUtc = now
                };

                _dbContext.TicketMessages.Add(adminMessage);

                ticket.Status = TicketStatus.WaitingForCustomer;
                ticket.AssignedAdminEmail = adminEmail;
                ticket.LastMessageAtUtc = now;

                await _dbContext.SaveChangesAsync();

                var mailSubject = $"[{ticket.TicketNo}] {ticket.Subject}";
                var mailBody = BuildCustomerReplyBody(ticket, message);

                await _emailSender.SendAsync(
                    ticket.CustomerEmail,
                    mailSubject,
                    mailBody,
                    lastExternalMessageId);

                transaction.Commit();
                result.SetData(true);
                result.SetMessage("Ýþlem baþarý ile gerçekleþtirildi.");

            }
            catch (Exception ex)
            {
                transaction.Rollback();

                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
        }

        return result;
    }

    private static string BuildCustomerReplyBody(Ticket ticket, string adminMessage)
    {
        return $"""
                Hello {ticket.CustomerName},

                Support team replied to your request.

                Ticket No: {ticket.TicketNo}
                Subject: {ticket.Subject}

                Message:
                {adminMessage}

                Please reply to this email to continue the conversation.

                Regards,
                Support Team
                """;
    }
}
