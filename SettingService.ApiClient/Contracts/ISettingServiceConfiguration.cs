namespace SettingService.ApiClient.Contracts;

public interface ISettingServiceConfiguration
{
    string ApplicationName { get; }

    bool UseRabbit { get; }

    RabbitConnectionParams? RabbitConnectionParams { get; }
}
