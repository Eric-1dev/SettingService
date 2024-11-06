using Microsoft.Extensions.DependencyInjection;

namespace SettingService.ApiClient.DI;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSettingServiceClient(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient<ISettingServiceClient, SettingServiceClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
