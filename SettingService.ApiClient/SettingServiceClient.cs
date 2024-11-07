
using EasyNetQ;
using SettingService.ApiClient.Contracts;
using SettingService.Contracts;

namespace SettingService.ApiClient;

internal class SettingServiceClient : ISettingServiceClient
{
    private readonly ISettingServiceConfiguration _configuration;
    private readonly IWebApiClient _webApiClient;

    public SettingServiceClient(ISettingServiceConfiguration configuration, IWebApiClient webApiClient)
    {
        _configuration = configuration;
        _webApiClient = webApiClient;
    }

    public async Task<IReadOnlyCollection<SettingItem>> Start()
    {
        ValidateSettings();

        if (_configuration.UseRabbit)
        {
            InitializeBus(_configuration.RabbitConnectionParams);
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

    private async Task InitializeBus(RabbitConnectionParams rabbitConfig)
    {
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


    }
}
