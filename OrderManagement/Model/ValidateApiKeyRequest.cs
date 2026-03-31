namespace OrderManagement.Model
{
    public class ValidateApiKeyRequest
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
    }
}
