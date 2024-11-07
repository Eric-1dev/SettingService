using Microsoft.Extensions.DependencyInjection;
using SettingService.ApiClient.Contracts;
using SettingService.ApiClient.DI;

var services = new ServiceCollection();

services.AddSettingServiceClient();

var provider = services.BuildServiceProvider();

var apiClient = provider.GetRequiredService<ISettingServiceClient>();

var settings = await apiClient.Start();

Console.ReadLine();