using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Entity;

public class OrderProduct
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public Product? Product { get; set; }
    public string? ProductValue { get; set; }
    public long OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int OrderStatus { get; set; }
    public DateTime? ProccessDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public int NumberOf { get; set; } = 1;
    public string? AoiId { get; set; }
    public string? AoiName { get; set; }
    public string? AoiWkt { get; set; }
    public string? RequestWkt { get; set; }
    public string? IntersectionWkt { get; set; }
    public double RequestAreaKm2 { get; set; }
    public double UnitPrice { get; set; }
    public double BaseTotalPrice { get; set; }
    public string ProcessingOptionsJson { get; set; } = "[]";
    [NotMapped] public object? ProcessingOptions { get; set; }
    public double ProcessingTotalPrice { get; set; }
    public double CalculatedTotalPrice { get; set; }
    public string ItemType { get; set; } = "satelliteImage";
}
