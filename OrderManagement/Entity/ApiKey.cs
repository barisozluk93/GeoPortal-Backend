using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Entity
{
    public class ApiKey
    {
        public long Id { get; set; }
        public string Key { get; set; } = null!;
        public long UserId { get; set; }
        public long OrderId { get; set; }
        public long OrderProductId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }

        [ForeignKey(nameof(OrderProductId))]
        public OrderProduct? OrderProduct { get; set; }

        public ICollection<ApiKeyPermission>? Permissions { get; set; }
    }
}
