using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Entity;

public class BasketProduct
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public long BasketId { get; set; }
    public Basket Basket { get; set; } = null!;
    public int NumberOf { get; set; } = 1;
    public string? AoiId { get; set; }
    public string? AoiName { get; set; }
    public string? AoiWkt { get; set; }
    public string? RequestWkt { get; set; }
    public string? RequestHash { get; set; }
    public string? IntersectionWkt { get; set; }
    public double RequestAreaKm2 { get; set; }
    public double UnitPrice { get; set; }
    public double BaseTotalPrice { get; set; }
    public string ProcessingOptionsJson { get; set; } = "[]";
    public double ProcessingTotalPrice { get; set; }
    public double CalculatedTotalPrice { get; set; }
    public string ItemType { get; set; } = "satelliteImage";
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
}
