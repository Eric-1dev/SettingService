namespace SettingService.ApiClient.Contracts;

internal class DefaultSettingServiceConfiguration : ISettingServiceConfiguration
{
    public string ApplicationName => null!;

    public bool UseRabbit => false;

    public RabbitConnectionParams? RabbitConnectionParams => null;
}
