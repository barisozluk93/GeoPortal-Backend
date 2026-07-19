namespace AuditLog.Shared;

public sealed class AuditLogEntry
{
    public DateTime TimestampUtc { get; set; }
    public string ServiceName { get; set; } = null!;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Roles { get; set; }
    public string ActionType { get; set; } = null!;
    public string HttpMethod { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string? QueryString { get; set; }
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public long DurationMs { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string CorrelationId { get; set; } = null!;
    public string? ErrorMessage { get; set; }
}
