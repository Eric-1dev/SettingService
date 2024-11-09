using Microsoft.Extensions.Logging;
using SettingService.ApiClient.Contracts;
using SettingService.ApiClient.Interfaces;
using SettingService.Contracts;

namespace SettingService.ApiClient.Services;

internal class SettingServiceClient : ISettingServiceClient
{
    private static bool _isRunning = false;
    private static readonly object _locker = new();
    private static List<SettingItem> _settings = [];

    private readonly ISettingServiceConfiguration _configuration;
    private readonly IWebApiClient _webApiClient;
    private readonly IRabbitIntegrationService _rabbitIntegrationService;
    private readonly ILogger _logger;

    public SettingServiceClient(ISettingServiceConfiguration configuration,
        IWebApiClient webApiClient,
        IRabbitIntegrationService rabbitIntegrationService,
        ILogger<SettingServiceClient> logger)
    {
        _configuration = configuration;
        _webApiClient = webApiClient;
        _rabbitIntegrationService = rabbitIntegrationService;
        _logger = logger;
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        ThrowIfRunning();

        ValidateSettings();

        if (_configuration.UseRabbit)
        {
            await _rabbitIntegrationService.InitializeBus(_configuration.RabbitConnectionParams!, _configuration.ApplicationName, OnMessage, cancellationToken);
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

    private async Task OnMessage(RabbitMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Изменена настройка {settingName}. Запрашиваю новое значение", message.SettingName);

        SettingItem? settingItem = null;
        if (message.ChangeTypeEnum != SettingChangeTypeEnum.Removed)
        {
            settingItem = await _webApiClient.GetByName(_configuration.ApplicationName, message.SettingName);

            if (settingItem == null)
            {
                _logger.LogWarning("Не удалось получить настройку {settingName} из сервиса", message.SettingName);
                return;
            }
        }

        HandleChangeType(message, settingItem);
    }

    private void HandleChangeType(RabbitMessage message, SettingItem? settingItem)
    {
        switch (message.ChangeTypeEnum)
        {
            case SettingChangeTypeEnum.Added:
                {
                    _settings.Add(settingItem!);
                    _logger.LogInformation("Добавлена новая настройка {settingName}", message.SettingName);
                    break;
                }
            case SettingChangeTypeEnum.Removed:
                {
                    var existingItem = _settings.FirstOrDefault(x => x.Name == message.SettingName);
                    if (existingItem == null)
                    {
                        _logger.LogWarning("Не найдена существующая настройка {settingName}. Удаление не требуется.", message.SettingName);
                    }
                    else
                    {
                        _settings.Remove(existingItem);
                        _logger.LogInformation("Настройка удалена {settingName}", message.SettingName);
                    }
                    break;
                }
            case SettingChangeTypeEnum.Changed:
                {
                    var nameToFind = message.OldSettingName ?? message.SettingName;

                    var existingItem = _settings.FirstOrDefault(x => x.Name == nameToFind);
                    if (existingItem == null)
                    {
                        _settings.Add(settingItem!);
                        _logger.LogWarning("Не найдена существующая настройка {settingName}. Добавляю.", message.SettingName);
                    }
                    else
                    {
                        existingItem.Value = settingItem!.Value;

                        if (existingItem.Name != message.SettingName)
                        {
                            var oldName = existingItem.Name;
                            existingItem.Name = message.SettingName;
                            _logger.LogInformation("Значение настройки {oldName} обновлено. Настройка переименована в {newName}", oldName, message.SettingName);
                        }
                        else
                        {
                            _logger.LogInformation("Значение настройки {settingName} обновлено", message.SettingName);
                        }
                    }
                    break;
                }
        }
    }
}