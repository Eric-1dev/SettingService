﻿using Microsoft.Extensions.DependencyInjection;
using SettingService.ApiClient.Contracts;
using SettingService.ApiClient.Interfaces;
using SettingService.ApiClient.Services;
using SettingService.Cache;

namespace SettingService.ApiClient.DI;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSettingServiceClient(this IServiceCollection services)
    {
        services.AddCache();

        services.AddSingleton<IEncryptionService, EncryptionService>();

        services.AddHttpClient<IWebApiClient, WebApiClient>(client =>
        {
            //client.DefaultRequestHeaders.Authorization
        });

        services.AddScoped<IRabbitIntegrationService, RabbitIntegrationService>();

        services.AddScoped<ISettingServiceClient, SettingServiceClient>();

        return services;
    }

    public static IServiceCollection AddSettingServiceClient(this IServiceCollection services, Action<ISettingServiceConfiguration> configure)
    {
        services.AddScoped<ISettingServiceConfiguration>(serviceProvider =>
        {
            var config = new DefaultSettingServiceConfiguration();
            configure(config);

            return config;
        });

        services.AddSettingServiceClient();

        return services;
    }
}
