using Microsoft.Extensions.DependencyInjection;
using SettingService.ApiClient;
using SettingService.ApiClient.DI;

var services = new ServiceCollection();

services.AddSettingServiceClient("http://localhost:5000");

var provider = services.BuildServiceProvider();

var apiClient = provider.GetRequiredService<ISettingServiceClient>();

var settings = await apiClient.GetAllSettings("CCS");

Console.ReadLine();