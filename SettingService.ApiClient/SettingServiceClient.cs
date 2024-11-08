
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using SettingService.ApiClient.Contracts;
using SettingService.Contracts;

namespace SettingService.ApiClient;

internal class SettingServiceClient : ISettingServiceClient
{
    private readonly ISettingServiceConfiguration _configuration;
    private readonly IWebApiClient _webApiClient;
    private readonly ILogger _logger;

    public SettingServiceClient(ISettingServiceConfiguration configuration, IWebApiClient webApiClient, ILogger<SettingServiceClient> logger)
    {
        _configuration = configuration;
        _webApiClient = webApiClient;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<SettingItem>> Start(CancellationToken cancellationToken)
    {
        ValidateSettings();

        if (_configuration.UseRabbit)
        {
            await InitializeBus(_configuration, cancellationToken);
        }

        var settings = await _webApiClient.GetAll(_configuration.ApplicationName);

        return settings;
    }

    private void ValidateSettings()
    {
        if (_configuration == null)
            throw new Exception($"Не найдена конфигурация. Требуется реализация интерфейса {nameof(ISettingServiceConfiguration)} или задайте параметры при регистрации в DI");

        if (string.IsNullOrEmpty(_configuration.ApplicationName))
            throw new Exception($"Не задано имя приложения");

        if (_configuration.UseRabbit && _configuration.RabbitConnectionParams == null)
            throw new Exception($"Не заданы параметры подключения к RabbitMQ");
    }

    private async Task InitializeBus(ISettingServiceConfiguration config, CancellationToken cancellationToken)
    {
        var rabbitConfig = _configuration.RabbitConnectionParams!;

        var bus = RabbitHutch.CreateBus(serviceResolver => {
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
        var queueName = $"setting_serive.{config.ApplicationName}.${identifier}-q";
        var queue = await bus.Advanced.QueueDeclareAsync(queueName, durable: false, exclusive: true, autoDelete: true);

        await bus.Advanced.BindAsync(exchange, queue, $"setting-service.{config.ApplicationName}.*");

        bus.Advanced.Consume<RabbitMessage>(queue, (message, messageReceivedInfo) => OnMessage(message, messageReceivedInfo, cancellationToken));
    }

    private async Task OnMessage(IMessage<RabbitMessage> message, MessageReceivedInfo messageReceivedInfo, CancellationToken cancellationToken)
    {
        var body = message.Body;

        _logger.LogInformation("Изменено значение настройки {settingName}", body.SettingName);
    }
}
