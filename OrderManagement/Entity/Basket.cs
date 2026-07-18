using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Entity;

public class Basket
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    [NotMapped] public long BasketItemId { get; set; }
    [NotMapped] public long ProductId { get; set; }
    [NotMapped] public Product? Product { get; set; }
    [NotMapped] public int NumberOf { get; set; } = 1;
    [NotMapped] public string? AoiId { get; set; }
    [NotMapped] public string? AoiName { get; set; }
    [NotMapped] public string? AoiWkt { get; set; }
    [NotMapped] public string? RequestWkt { get; set; }
    [NotMapped] public string? IntersectionWkt { get; set; }
    [NotMapped] public double RequestAreaKm2 { get; set; }
    [NotMapped] public double UnitPrice { get; set; }
    [NotMapped] public double BaseTotalPrice { get; set; }
    [NotMapped] public string ProcessingOptionsJson { get; set; } = "[]";
    [NotMapped] public object? ProcessingOptions { get; set; }
    [NotMapped] public double ProcessingTotalPrice { get; set; }
    [NotMapped] public double CalculatedTotalPrice { get; set; }
    [NotMapped] public string ItemType { get; set; } = "satelliteImage";
}
