using EasyNetQ;
using EasyNetQ.Topology;
using SettingService.ApiClient.Contracts;
using SettingService.ApiClient.Interfaces;
using SettingService.Contracts;

namespace SettingService.ApiClient.Services;

internal class RabbitIntegrationService : IRabbitIntegrationService
{
    public async Task InitializeBus(RabbitConnectionParams rabbitConfig,
        string applicationName,
        Func<RabbitMessage, CancellationToken, Task> onMessageReceived,
        CancellationToken cancellationToken)
    {
        var bus = RabbitHutch.CreateBus(serviceResolver =>
        {
            return new ConnectionConfiguration
            {
                Hosts = [
                        new HostConfiguration {
                            Host = rabbitConfig.HostName,
                            Port = rabbitConfig.Port,
                        }
                    ],
                VirtualHost = rabbitConfig.VirtualHost,
                UserName = rabbitConfig.UserName,
                Password = rabbitConfig.Password,
            };
        }, reg => { });

        var exchange = await bus.Advanced.ExchangeDeclareAsync("setting-service-ex", type: ExchangeType.Topic, durable: true, autoDelete: false);

        var identifier = Guid.NewGuid().ToString();
        var queueName = $"setting_service.{applicationName}.{identifier}-q";
        var queue = await bus.Advanced.QueueDeclareAsync(queueName, durable: false, exclusive: true, autoDelete: true);

        await bus.Advanced.BindAsync(exchange, queue, $"#.{applicationName}.#");

        bus.Advanced.Consume<RabbitMessage>(queue, (message, messageReceivedInfo) => onMessageReceived(message.Body, cancellationToken));
    }
}
