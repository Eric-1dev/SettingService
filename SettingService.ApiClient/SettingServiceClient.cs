using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using SettingService.ApiClient.Contracts;
using SettingService.Contracts;

namespace SettingService.ApiClient;

internal class SettingServiceClient : ISettingServiceClient
{
    private static bool _isRunning = false;
    private static readonly object _locker = new();
    private static List<SettingItem> _settings = [];

    private readonly ISettingServiceConfiguration _configuration;
    private readonly IWebApiClient _webApiClient;
    private readonly ILogger _logger;

    public SettingServiceClient(ISettingServiceConfiguration configuration, IWebApiClient webApiClient, ILogger<SettingServiceClient> logger)
    {
        _configuration = configuration;
        _webApiClient = webApiClient;
        _logger = logger;
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        ThrowIfRunning();

        ValidateSettings();

        if (_configuration.UseRabbit)
        {
            await InitializeBus(_configuration, cancellationToken);
        }

        var settings = await _webApiClient.GetAll(_configuration.ApplicationName);

        _settings = [.. settings];
    }

    public string? GetStringSetting(string settingName)
    {
        var setting = GetSetting(settingName);

        if (setting?.ValueType == SettingValueTypeEnum.String)
            return setting.Value;

        return null;
    }

    public int? GetIntSetting(string settingName)
    {
        var setting = GetSetting(settingName);

        if (setting?.ValueType == SettingValueTypeEnum.Int)
            return int.Parse(setting.Value);

        return null;
    }

    public long? GetLongSetting(string settingName)
    {
        var setting = GetSetting(settingName);

        if (setting?.ValueType == SettingValueTypeEnum.Long)
            return long.Parse(setting.Value);

        return null;
    }

    public double? GetDoubleSetting(string settingName)
    {
        var setting = GetSetting(settingName);

        if (setting?.ValueType == SettingValueTypeEnum.Double)
            return double.Parse(setting.Value);

        return null;
    }

    public decimal? GetDecimalSetting(string settingName)
    {
        var setting = GetSetting(settingName);

        if (setting?.ValueType == SettingValueTypeEnum.Decimal)
            return decimal.Parse(setting.Value);

        return null;
    }

    public bool? GetBoolSetting(string settingName)
    {
        var setting = GetSetting(settingName);

        if (setting?.ValueType == SettingValueTypeEnum.Bool)
            return bool.Parse(setting.Value);

        return null;
    }

    private SettingItem? GetSetting(string settingName)
    {
        var setting = _settings.FirstOrDefault(x => x.Name == settingName);

        return setting;
    }

    private void ValidateSettings()
    {
        if (_configuration == null)
            throw new Exception($"Не найдена конфигурация. Требуется реализация интерфейса {nameof(ISettingServiceConfiguration)} или задать параметры при регистрации в DI");

        if (string.IsNullOrEmpty(_configuration.ApplicationName))
            throw new Exception($"Не задано имя приложения");

        if (_configuration.UseRabbit && _configuration.RabbitConnectionParams == null)
            throw new Exception($"Не заданы параметры подключения к RabbitMQ");
    }

    private async Task InitializeBus(ISettingServiceConfiguration config, CancellationToken cancellationToken)
    {
        var rabbitConfig = _configuration.RabbitConnectionParams!;

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
        var queueName = $"setting_service.{config.ApplicationName}.{identifier}-q";
        var queue = await bus.Advanced.QueueDeclareAsync(queueName, durable: false, exclusive: true, autoDelete: true);

        await bus.Advanced.BindAsync(exchange, queue, $"#.{config.ApplicationName}.#");

        bus.Advanced.Consume<RabbitMessage>(queue, (message, messageReceivedInfo) => OnMessage(message, messageReceivedInfo, cancellationToken));
    }

    private void ThrowIfRunning()
    {
        if (_isRunning)
            throw new InvalidOperationException("Клиент сервиса настроек уже проинициализирован");

        lock (_locker)
        {
            if (_isRunning)
                throw new InvalidOperationException("Клиент сервиса настроек уже проинициализирован");

            _isRunning = true;
        }
    }

    private async Task OnMessage(IMessage<RabbitMessage> message, MessageReceivedInfo messageReceivedInfo, CancellationToken cancellationToken)
    {
        var body = message.Body;

        _logger.LogInformation("Изменена настройка {settingName}. Запрашиваю новое значение", body.SettingName);

        SettingItem? settingItem = null;
        if (body.ChangeTypeEnum != SettingChangeTypeEnum.Removed)
        {
            settingItem = await _webApiClient.GetByName(_configuration.ApplicationName, body.SettingName);

            if (settingItem == null)
            {
                _logger.LogWarning("Не удалось получить настройку {settingName} из сервиса", body.SettingName);
                return;
            }
        }

        HandleChangeType(body.ChangeTypeEnum, body.SettingName, settingItem);
    }

    private void HandleChangeType(SettingChangeTypeEnum changeType, string settingName, SettingItem? settingItem)
    {
        switch (changeType)
        {
            case SettingChangeTypeEnum.Added:
                {
                    _settings.Add(settingItem!);
                    break;
                }
            case SettingChangeTypeEnum.Removed:
                {
                    var existingValue = _settings.FirstOrDefault(x => x.Name == settingName);
                    if (existingValue == null)
                    {
                        _logger.LogWarning("Не найдена существующая настройка {settingName}. Удаление не требуется.", settingName);
                    }
                    else
                    {
                        _settings.Remove(existingValue);
                        _logger.LogInformation("Настройка удалена {settingName}", settingName);
                    }
                    break;
                }
            case SettingChangeTypeEnum.Changed:
                {
                    var existingValue = _settings.FirstOrDefault(x => x.Name == settingName);
                    if (existingValue == null)
                    {
                        _settings.Add(settingItem!);
                        _logger.LogWarning("Не найдена существующая настройка {settingName}. Добавляю.", settingName);
                    }
                    else
                    {
                        existingValue.Value = settingItem!.Value;
                        _logger.LogInformation("Значение настройки {settingName} обновлено", settingName);
                    }
                    break;
                }
        }
    }
}