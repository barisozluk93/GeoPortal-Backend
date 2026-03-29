using System.ComponentModel.DataAnnotations.Schema;

namespace MapManagement.Entity
{
    public class LayerGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public List<Layer> Layers { get; set; } = new List<Layer>();
    }
}
