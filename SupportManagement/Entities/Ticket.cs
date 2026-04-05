namespace SupportManagement.Entities;

public class Ticket
{
    public long Id { get; set; }
    public string TicketNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? Organization { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime LastMessageAtUtc { get; set; }
    public bool IsClosed { get; set; }
    public string? AssignedAdminEmail { get; set; }
    public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
}
