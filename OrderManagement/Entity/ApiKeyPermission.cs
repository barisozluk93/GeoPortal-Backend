using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Entity
{
    public class ApiKeyPermission
    {
        public long Id { get; set; }
        public long ApiKeyId { get; set; }
        public string Endpoint { get; set; } = null!;

        [ForeignKey(nameof(ApiKeyId))]
        public ApiKey? ApiKey { get; set; }
    }
}
