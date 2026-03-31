namespace MapManagement.Model
{
    public class ValidateApiKeyRequest
    {
        public string ApiKey { get; set; } = null!;
        public string Endpoint { get; set; } = null!;
    }
}
