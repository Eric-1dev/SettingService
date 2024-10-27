using Microsoft.Extensions.DependencyInjection;

namespace SettingService.ApiClient.DI;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSettingServiceClient(this IServiceCollection services)
    {
        services.AddScoped<ISettingServiceClient, SettingServiceClient>();

        return services;
    }
}
