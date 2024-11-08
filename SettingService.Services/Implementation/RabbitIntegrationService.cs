using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;
using SettingService.Services.Interfaces;
using SettingService.Services.Models;

namespace SettingService.Services.Implementation;

internal class RabbitIntegrationService : IRabbitIntegrationService
{
    private readonly IOptions<RabbitConfig> _config;

    public RabbitIntegrationService(IOptions<RabbitConfig> config)
    {
        _config = config;
    }

    public async Task Initialize()
    {
        var bus = RabbitHutch.CreateBus(serviceResolver => {
            return new ConnectionConfiguration
            {
                Hosts = [
                        new HostConfiguration {
                            Host = _config.Value.HostName,
                            Port = _config.Value.Port,
                        }
                    ],
                VirtualHost = _config.Value.VirtualHost,
                UserName = _config.Value.UserName,
                Password = _config.Value.Password,
            };
        }, reg => { });

        var exchange = await bus.Advanced.ExchangeDeclareAsync("setting-service-ex", type: ExchangeType.Topic, durable: true, autoDelete: false);

        var queueName = $"setting_serive.server.${Guid.NewGuid()}-q";
        var queue = await bus.Advanced.QueueDeclareAsync(queueName, durable: false, exclusive: true, autoDelete: true);

        await bus.Advanced.BindAsync(exchange, queue, $"setting-service.server.*");
    }
}
