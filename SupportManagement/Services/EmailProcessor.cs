using Microsoft.EntityFrameworkCore;
using MimeKit;
using SupportManagement.DbContexts;
using SupportManagement.Entities;
using SupportManagement.Enums;
using SupportManagement.Interfaces;
using System.Text.RegularExpressions;

namespace SupportManagement.Services;

public class EmailProcessor : IEmailProcessor
{
    private readonly SupportManagementDbContext _dbContext;
    private readonly ILogger<EmailProcessor> _logger;

    public EmailProcessor(SupportManagementDbContext dbContext, ILogger<EmailProcessor> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ProcessAsync(MimeMessage message)
    {
        var externalMessageId = message.MessageId;
        if (string.IsNullOrWhiteSpace(externalMessageId))
            return;

        var alreadyExists = await _dbContext.TicketMessages
            .AnyAsync(x => x.ExternalMessageId == externalMessageId);

        if (alreadyExists)
            return;

        var subject = message.Subject ?? string.Empty;
        var fromEmail = message.From.Mailboxes.FirstOrDefault()?.Address;
        var body = message.TextBody ?? message.HtmlBody ?? string.Empty;

        if (string.IsNullOrWhiteSpace(fromEmail))
            return;

        var ticketNo = ExtractTicketNo(subject);
        if (string.IsNullOrWhiteSpace(ticketNo))
        {
            _logger.LogWarning("Ticket number could not be extracted from subject: {Subject}", subject);
            return;
        }

        var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(x => x.TicketNo == ticketNo);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket not found for TicketNo: {TicketNo}", ticketNo);
            return;
        }

        var now = DateTime.UtcNow;
        var newMessage = new TicketMessage
        {
            TicketId = ticket.Id,
            SenderType = SenderType.Customer,
            Channel = MessageChannel.Email,
            SenderEmail = fromEmail,
            Body = CleanBody(body),
            ExternalMessageId = externalMessageId,
            InReplyToExternalMessageId = message.InReplyTo,
            CreatedAtUtc = now
        };

        _dbContext.TicketMessages.Add(newMessage);
        ticket.Status = TicketStatus.CustomerReplied;
        ticket.LastMessageAtUtc = now;
        ticket.IsClosed = false;

        await _dbContext.SaveChangesAsync();
    }

    private static string? ExtractTicketNo(string subject)
    {
        var match = Regex.Match(subject, @"CT-\d{4}-\d{6}");
        return match.Success ? match.Value : null;
    }

    private static string CleanBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return string.Empty;

        var separators = new[]
        {
            "\nOn ",
            "\nFrom:",
            "\n-----Original Message-----",
            "\n________________________________"
        };

        foreach (var separator in separators)
        {
            var index = body.IndexOf(separator, StringComparison.OrdinalIgnoreCase);
            if (index > 0)
            {
                body = body[..index];
                break;
            }
        }

        return body.Trim();
    }
}
