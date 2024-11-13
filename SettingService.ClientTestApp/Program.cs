using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SettingService.ApiClient.Contracts;
using SettingService.ApiClient.DI;

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

services.AddLogging(builder =>
{
    builder.ClearProviders();

    var logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

    builder.AddSerilog(logger);
});

var provider = services.BuildServiceProvider();

var settingServiceClient = provider.GetRequiredService<ISettingServiceClient>();

await Task.Delay(TimeSpan.FromSeconds(3));

await settingServiceClient.Start(CancellationToken.None);

Console.ReadLine();