using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Model
{
    public class Layer
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long LayerGroupId { get; set; }
        public string Url { get; set; }
        public bool IsBaseMap { get; set; }

        public bool IsDeleted { get; set; }
        public double? Price { get; set; }

    }
}
