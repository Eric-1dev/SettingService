using SettingService.ApiClient.Contracts;
using SettingService.Contracts;

namespace SettingService.ApiClient.Interfaces;

internal interface IRabbitIntegrationService
{
    Task InitializeBus(RabbitConnectionParams rabbitConfig,
        string applicationName,
        Func<RabbitMessage, CancellationToken, Task> onMessageReceived,
        CancellationToken cancellationToken);
}
