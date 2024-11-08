namespace SettingService.ApiClient.Contracts;

public interface ISettingServiceConfiguration
{
    string SettingServiceUrl { get; set; }

    string ApplicationName { get; set; }

    bool UseRabbit { get; set; }

    RabbitConnectionParams? RabbitConnectionParams { get; set; }
}
