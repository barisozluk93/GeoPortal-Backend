namespace OrderManagement.Model
{
    public class ValidateApiKeyResponse
    {
        public bool IsValid { get; set; }
        public long? ApiKeyId { get; set; }
        public long? UserId { get; set; }
        public long? OrderId { get; set; }
        public long? OrderProductId { get; set; }
        public string? Error { get; set; }
    }
}
