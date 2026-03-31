using Microsoft.AspNetCore.Mvc;
using OrderManagement.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Entity
{
    public class OrderProduct
    {
        public long Id { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        public long ProductId { get; set; }
        public string? ProductValue { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public long OrderId { get; set; }
        public int OrderStatus { get; set; }
        public DateTime? ProccessDate { get; set; }
        public long? FileId { get; set; }
        public DateTime? CompletionDate { get; set; }

        [NotMapped]
        public FileContentResult? FileResult { get; set; }

        [NotMapped]
        public string? FileName { get; set; }

    }
}
