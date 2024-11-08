using SettingService.Services.Interfaces;

namespace SettingService.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseRabbit(this WebApplication app)
    {
        var rabbitIntegrationService = app.Services.GetRequiredService<IRabbitIntegrationService>();

        rabbitIntegrationService.Initialize().Wait();

        return app;
    }
}
