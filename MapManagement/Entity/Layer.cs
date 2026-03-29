using System.ComponentModel.DataAnnotations.Schema;

namespace MapManagement.Entity
{
    public class Layer
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long LayerGroupId { get; set; }

        [ForeignKey("LayerGroupId")]
        public LayerGroup LayerGroup { get; set; }
        public string Url { get; set; }
        public bool IsBaseMap { get; set; }

        public bool IsDeleted { get; set; }
        public double? Price { get; set; }

    }
}
