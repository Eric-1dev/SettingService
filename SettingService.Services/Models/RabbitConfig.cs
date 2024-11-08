namespace SettingService.Services.Models;

public class RabbitConfig
{
    public string HostName { get; set; }

    public ushort Port { get; set; }

    public string VirtualHost { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public RabbitConfig()
    {
        HostName = string.Empty;
        Port = 0;
        VirtualHost = string.Empty;
        UserName = string.Empty;
        Password = string.Empty;
    }
}
