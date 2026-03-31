namespace MapManagement.Interfaces
{
    public interface IApiKeyValidationService
    {
        Task<ApiKeyValidationResult> ValidateAsync(string apiKey, string endpoint, CancellationToken cancellationToken = default);
    }

    public class ApiKeyValidationResult
    {
        public bool IsValid { get; set; }
        public int? ApiKeyId { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
        public int? OrderProductId { get; set; }
        public string? Error { get; set; }
    }
}
