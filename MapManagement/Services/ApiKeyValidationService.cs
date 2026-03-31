using System.Text;
using System.Text.Json;
using MapManagement.Interfaces;
using MapManagement.Model;

namespace MapManagement.Services
{
    public class ApiKeyValidationService : IApiKeyValidationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyValidationService> _logger;

        public ApiKeyValidationService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ApiKeyValidationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ApiKeyValidationResult> ValidateAsync(string apiKey, string endpoint, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("OrderManagementValidation");
                using var request = new HttpRequestMessage(HttpMethod.Post, _configuration["AppSettings:ApiUrl"] + "/geoPortalApi/InternalApikey/Validate");

                var internalSecret = _configuration["OrderManagementValidation:InternalSecret"];
                if (!string.IsNullOrWhiteSpace(internalSecret))
                {
                    request.Headers.Add("X-Internal-Secret", internalSecret);
                }

                var payload = new ValidateApiKeyRequest
                {
                    ApiKey = apiKey,
                    Endpoint = endpoint
                };

                request.Content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json");

                using var response = await client.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Api key validation request failed. StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, content);
                    return new ApiKeyValidationResult
                    {
                        IsValid = false,
                        Error = "Authorization service unavailable"
                    };
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var resultWrapper = JsonSerializer.Deserialize<Result<ValidateApiKeyResponse>>(content, options);
                var data = resultWrapper?.GetData();

                if (data == null)
                {
                    _logger.LogWarning("Api key validation response could not be parsed. Response: {Response}", content);
                    return new ApiKeyValidationResult
                    {
                        IsValid = false,
                        Error = "Authorization response invalid"
                    };
                }

                return new ApiKeyValidationResult
                {
                    IsValid = data.IsValid,
                    ApiKeyId = data.ApiKeyId,
                    UserId = data.UserId,
                    OrderId = data.OrderId,
                    OrderProductId = data.OrderProductId,
                    Error = data.Error
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Api key validation request threw an exception.");
                return new ApiKeyValidationResult
                {
                    IsValid = false,
                    Error = "Authorization service unavailable"
                };
            }
        }
    }
}
