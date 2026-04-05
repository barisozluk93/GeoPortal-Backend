namespace SupportManagement.Entities;

public class TicketMessage
{
    public long Id { get; set; }
    public long TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public string SenderType { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string? SenderEmail { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? ExternalMessageId { get; set; }
    public string? InReplyToExternalMessageId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
