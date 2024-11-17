using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SettingService.Contracts;
using SettingService.Services.Interfaces;
using SettingService.Services.Models;
using SettingService.Cache;

namespace SettingService.Services.Implementation;

internal class RabbitIntegrationService : IRabbitIntegrationService
{
    private static IBus? _bus;
    private static IExchange? _exchange;
    private static readonly string _serverQueueName = $"setting_service.server.{Guid.NewGuid()}-q";
    
    private readonly IOptions<RabbitConfig> _config;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger _logger;
    private readonly ICacheService _cacheService;

    public RabbitIntegrationService(IOptions<RabbitConfig> config,
        IEncryptionService encryptionService,
        ILogger<RabbitIntegrationService> logger,
        ICacheService cacheService)
    {
        _config = config;
        _encryptionService = encryptionService;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Initialize()
    {
        _bus = RabbitHutch.CreateBus(serviceResolver =>
        {
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
                PublisherConfirms = true,
            };
        }, reg => { });

        _bus.Advanced.Conventions.ConsumerTagConvention = () => Environment.MachineName;

        _exchange = await _bus.Advanced.ExchangeDeclareAsync("setting-service-ex", type: ExchangeType.Topic, durable: true, autoDelete: false);

        var queue = await _bus.Advanced.QueueDeclareAsync(_serverQueueName, durable: false, exclusive: true, autoDelete: true);

        await _bus.Advanced.BindAsync(_exchange, queue, "setting-service-server.#");

        _bus.Advanced.Consume<RabbitMessage>(queue, async (message, messageInfo) =>
        {
            await HandleRabbitMessage(message.Body);
        });
    }

    public async Task PublishChange(SettingItem settingItem, string[] applicationNames, SettingChangeTypeEnum changeType, string? oldName)
    {
        if (_bus == null)
            throw new Exception("Шина не проинициализирована");

        var encryptedValue = string.Empty;

        if (!string.IsNullOrEmpty(settingItem.Value))
            encryptedValue = _encryptionService.Encrypt(settingItem.Value!);

        var message = new Message<RabbitMessage>(new RabbitMessage
        {
            ChangeType = changeType,
            OldSettingName = oldName,
            CurrentName = settingItem.Name,
            EncryptedValue = encryptedValue,
            ValueType = settingItem.ValueType
        });

        var routingKey = $"setting-service-server.{string.Join(".", applicationNames)}.";

        await _bus.Advanced.PublishAsync(_exchange, routingKey, mandatory: true, message);
    }

    private Task HandleRabbitMessage(RabbitMessage message)
    {
        const string template = "Получено обновление настройки {settingName}. Тип обновления: {changeType}";
        _logger.LogInformation(template, message.OldSettingName ?? message.CurrentName, message.ChangeType);

        var decryptedValue = string.Empty;

        if (!string.IsNullOrEmpty(message.EncryptedValue))
            decryptedValue = _encryptionService.Decrypt(message.EncryptedValue!);

        var settingItem = new SettingItem
        {
            Name = message.CurrentName,
            Value = decryptedValue,
            ValueType = message.ValueType
        };

        _cacheService.HandleUpdate(message.ChangeType, settingItem, message.OldSettingName);

        return Task.CompletedTask;
    }
}
