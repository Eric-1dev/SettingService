namespace SettingService.ApiClient.Contracts;

public class RabbitConnectionParams
{
    public string HostName { get; set; }

    public ushort Port { get; set; }

    public string VirtualHost { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
}
