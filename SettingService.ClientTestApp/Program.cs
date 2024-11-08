using Microsoft.Extensions.DependencyInjection;
using SettingService.ApiClient.Contracts;
using SettingService.ApiClient.DI;
using SettingService.Contracts;

var services = new ServiceCollection();

services.AddSettingServiceClient(config =>
{
    config.SettingServiceUrl = "http://localhost:5186";
    config.ApplicationName = "CCS";
    config.UseRabbit = true;
    config.RabbitConnectionParams = new RabbitConnectionParams
    {
        HostName = "ryzen-pc",
        Port = 5672,
        VirtualHost = "/",
        UserName = "guest",
        Password = "guest",
    };
});

var provider = services.BuildServiceProvider();

var apiClient = provider.GetRequiredService<ISettingServiceClient>();

await Task.Delay(4000);

var settings = await apiClient.Start(CancellationToken.None);

Console.ReadLine();