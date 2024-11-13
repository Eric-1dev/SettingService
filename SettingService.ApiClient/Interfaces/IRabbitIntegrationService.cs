using SettingService.ApiClient.Contracts;
using SettingService.Contracts;

namespace SettingService.ApiClient.Interfaces;

internal interface IRabbitIntegrationService
{
    Task InitializeBus(RabbitConnectionParams rabbitConfig,
        string applicationName,
        Action<RabbitMessage, CancellationToken> onMessageReceived,
        CancellationToken cancellationToken);
}
