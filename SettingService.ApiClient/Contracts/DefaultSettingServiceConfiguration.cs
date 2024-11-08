namespace SettingService.ApiClient.Contracts;

internal class DefaultSettingServiceConfiguration : ISettingServiceConfiguration
{
    public string SettingServiceUrl { get; set; }

    public string ApplicationName { get; set; }

    public bool UseRabbit {  get; set; }

    public RabbitConnectionParams? RabbitConnectionParams { get; set; }

    public DefaultSettingServiceConfiguration()
    {
        SettingServiceUrl = null!;
        ApplicationName = null!;
        UseRabbit = false;
        RabbitConnectionParams = null;
    }
}
