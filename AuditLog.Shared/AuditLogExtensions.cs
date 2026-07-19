using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuditLog.Shared;

public static class AuditLogExtensions
{
    public static IServiceCollection AddGeoPortalAuditLogging(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        services.Configure<AuditLogOptions>(configuration.GetSection("AuditLog"));
        services.PostConfigure<AuditLogOptions>(o => o.ServiceName = serviceName);
        services.AddHttpClient("AuditLogClient", (sp, client) =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuditLogOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });
        return services;
    }

    public static IApplicationBuilder UseGeoPortalAuditLogging(this IApplicationBuilder app) => app.UseMiddleware<AuditLogMiddleware>();
}
