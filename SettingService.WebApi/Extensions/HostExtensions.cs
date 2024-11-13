using SettingService.Services.Interfaces;

namespace SettingService.WebApi.Extensions;

public static class HostExtensions
{
    public static IHost UseRabbit(this IHost app)
    {
        var scope = app.Services.CreateScope();

        var rabbitIntegrationService = scope.ServiceProvider.GetRequiredService<IRabbitIntegrationService>();
        var settingService = scope.ServiceProvider.GetRequiredService<ISettingsService>();

        Task.Run(async () =>
        {
            var isInitialized = false;

            while (!isInitialized)
            {
                try
                {
                    await rabbitIntegrationService.Initialize();

                    rabbitIntegrationService.Consume(settingService.HandleRabbitMessage);

                    isInitialized = true;
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        });

        return app;
    }
}
