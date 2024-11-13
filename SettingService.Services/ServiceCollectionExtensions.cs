using Microsoft.Extensions.DependencyInjection;
using SettingService.Services.Implementation;
using SettingService.Services.Implementation.ExternalSource;
using SettingService.Services.Implementation.UI;
using SettingService.Services.Interfaces;
using SettingService.Services.Interfaces.ExternalSource;
using SettingService.Services.Interfaces.UI;

namespace SettingService.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, CacheService>();
        
        services.AddScoped<IApplicationsUIService, ApplicationsUIService>();
        services.AddScoped<ISettingsUIService, SettingsUIService>();

        services.AddScoped<ISettingsService, SettingsService>();

        services.AddScoped<IExternalSourceService, VaultExternalSourceService>();
        //services.AddScoped<IExternalSourceService, OtherExternalSourceService>();

        return services;
    }

    public static IServiceCollection AddRabbit(this IServiceCollection services)
    {
        services.AddScoped<IRabbitIntegrationService, RabbitIntegrationService>();

        return services;
    }
}
