namespace AuditLogManagement.Model;
public sealed class AuditLogQuery
{
    public DateTime? StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? ServiceName { get; set; }
    public string? ActionType { get; set; }
    public bool? IsSuccess { get; set; }
    public string? CorrelationId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
