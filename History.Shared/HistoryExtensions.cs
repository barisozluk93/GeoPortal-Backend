using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace History.Shared;
public static class HistoryExtensions
{
    public static IServiceCollection AddGeoPortalHistory(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        services.Configure<HistoryOptions>(configuration.GetSection("History"));
        services.PostConfigure<HistoryOptions>(o => o.ServiceName = serviceName);
        services.AddHttpContextAccessor();
        services.AddHttpClient("HistoryManagement");
        services.AddScoped<HistorySaveChangesInterceptor>();
        return services;
    }
}
