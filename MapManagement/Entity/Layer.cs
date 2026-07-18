using MapManagement.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapManagement.Entity
{
    public class Layer
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public LayerType Type { get; set; }

        public string Url { get; set; } = null!;
        public string? LayerName { get; set; }
        public string? Format { get; set; }
        public string? Version { get; set; }

        public bool IsVisible { get; set; } = true;
        public double Opacity { get; set; } = 1.0;
        public int OrderNo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

    }
}
