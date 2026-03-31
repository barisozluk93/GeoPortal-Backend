using MapManagement.Authorization;
using MapManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MapManagement.Middleware
{
    public class ApiKeyValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyValidationMiddleware> _logger;

        public ApiKeyValidationMiddleware(RequestDelegate next, ILogger<ApiKeyValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IApiKeyValidationService apiKeyValidationService)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var allowAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null;
            if (allowAnonymous)
            {
                await _next(context);
                return;
            }

            var requireApiKey = endpoint.Metadata.GetMetadata<RequireApiKeyAttribute>() != null;
            if (!requireApiKey)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("x-api-key", out var apiKeyHeader) ||
                string.IsNullOrWhiteSpace(apiKeyHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API key missing");
                return;
            }

            var requestPath = context.Request.Path.Value?.Trim() ?? string.Empty;
            var validationResult = await apiKeyValidationService.ValidateAsync(
                apiKeyHeader.ToString().Trim(),
                requestPath,
                context.RequestAborted);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Api key validation failed for endpoint {Endpoint}. Error: {Error}",
                    requestPath,
                    validationResult.Error);

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync(validationResult.Error ?? "Forbidden");
                return;
            }

            context.Items["ApiKeyId"] = validationResult.ApiKeyId;
            context.Items["UserId"] = validationResult.UserId;
            context.Items["OrderId"] = validationResult.OrderId;
            context.Items["OrderProductId"] = validationResult.OrderProductId;

            await _next(context);
        }
    }
}