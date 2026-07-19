namespace HistoryManagement.Entities;
public sealed class HistoryRecord
{
    public long Id { get; set; }
    public string RecordId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? ChangesJson { get; set; }
}
