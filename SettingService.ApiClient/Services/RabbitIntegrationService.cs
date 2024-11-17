using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using SettingService.ApiClient.Contracts;
using SettingService.ApiClient.Interfaces;
using SettingService.Cache;
using SettingService.Contracts;

namespace SettingService.ApiClient.Services;

internal class RabbitIntegrationService : IRabbitIntegrationService
{
    private readonly ICacheService _cacheService;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger _logger;

    public RabbitIntegrationService(ICacheService cacheService,
        IEncryptionService encryptionService,
        ILogger<RabbitIntegrationService> logger)
    {
        _cacheService = cacheService;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    public async Task InitializeBus(RabbitConnectionParams rabbitConfig,
        string applicationName,
        string privateKey,
        CancellationToken cancellationToken)
    {
        _encryptionService.Initialize(privateKey);

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

        bus.Advanced.Conventions.ConsumerTagConvention = () => Environment.MachineName;

        var exchange = await bus.Advanced.ExchangeDeclareAsync("setting-service-ex", type: ExchangeType.Topic, durable: true, autoDelete: false);

        var identifier = Guid.NewGuid().ToString();
        var queueName = $"setting_service.{applicationName}.{identifier}-q";
        var queue = await bus.Advanced.QueueDeclareAsync(queueName, durable: false, exclusive: true, autoDelete: true);

        await bus.Advanced.BindAsync(exchange, queue, $"#.{applicationName}.#");

        bus.Advanced.Consume<RabbitMessage>(queue, (message, messageReceivedInfo) => OnMessage(message.Body, cancellationToken));
    }

    private void OnMessage(RabbitMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Изменена настройка {settingName}", message.CurrentName);

        string value = string.Empty;

        if (!string.IsNullOrEmpty(message.EncryptedValue))
            value = _encryptionService.Decrypt(message.EncryptedValue!);

        var settingItem = new SettingItem
        {
            Name = message.CurrentName,
            Value = value,
            ValueType = message.ValueType
        };

        _cacheService.HandleUpdate(message.ChangeType, settingItem, message.OldSettingName);
    }
}
