using SettingService.ApiClient.Contracts;

namespace SettingService.ApiClient.Interfaces;

internal interface IRabbitIntegrationService
{
    Task InitializeBus(RabbitConnectionParams rabbitConfig,
        string applicationName,
        string privateKey,
        CancellationToken cancellationToken);
}
