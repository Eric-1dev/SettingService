using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Options;
using SettingService.Contracts;
using SettingService.Services.Interfaces;
using SettingService.Services.Models;

namespace SettingService.Services.Implementation;

internal class RabbitIntegrationService : IRabbitIntegrationService
{
    private static IBus? _bus;
    private readonly IOptions<RabbitConfig> _config;
    private static IExchange? _exchange;

    public RabbitIntegrationService(IOptions<RabbitConfig> config)
    {
        _config = config;
    }

    public async Task Initialize()
    {
        _bus = RabbitHutch.CreateBus(serviceResolver => {
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

        _exchange = await _bus.Advanced.ExchangeDeclareAsync("setting-service-ex", type: ExchangeType.Topic, durable: true, autoDelete: false);

        var queueName = $"setting_service.server.{Guid.NewGuid()}-q";
        var queue = await _bus.Advanced.QueueDeclareAsync(queueName, durable: false, exclusive: true, autoDelete: true);

        await _bus.Advanced.BindAsync(_exchange, queue, "setting-service.server.*");
    }

    public async Task PublishChange(string name, string[] applicationNames, SettingChangeTypeEnum changeType, string? oldName)
    {
        if (_bus == null)
            throw new Exception("Шина не проинициализирована");

        var message = new Message<RabbitMessage>(new RabbitMessage
        {
            SettingName = name,
            ChangeTypeEnum = changeType,
            OldSettingName = oldName,
        });

        var routingKey = $".{string.Join(".", applicationNames)}.";

        await _bus.Advanced.PublishAsync(_exchange, routingKey, mandatory: true, message);
    }
}
