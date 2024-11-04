using Microsoft.Extensions.DependencyInjection;
using SettingService.Services.Implementation;
using SettingService.Services.Interfaces;

namespace SettingService.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddScoped<IApplicationsService, ApplicationsService>();
        services.AddScoped<ISettingsService, SettingsService>();

        return services;
    }
}
