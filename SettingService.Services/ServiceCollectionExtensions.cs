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
        services.AddScoped<IApplicationsUIService, ApplicationsUIService>();
        services.AddScoped<ISettingsUIService, SettingsUIService>();

        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IRabbitIntegrationService, RabbitIntegrationService>();

        services.AddScoped<IExternalSourceService, VaultExternalSourceService>();
        //services.AddScoped<IExternalSourceService, OtherExternalSourceService>();

        return services;
    }
}
