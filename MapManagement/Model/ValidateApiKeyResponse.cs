namespace MapManagement.Model
{
    public class ValidateApiKeyResponse
    {
        public bool IsValid { get; set; }
        public int? ApiKeyId { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
        public int? OrderProductId { get; set; }
        public string? Error { get; set; }
    }
}
