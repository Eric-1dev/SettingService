using SettingService.Contracts;

namespace SettingService.Services.Interfaces;

public interface IRabbitIntegrationService
{
    Task Initialize();
    Task PublishChange(string name, string[] applicationNames, SettingChangeTypeEnum changeType, string? oldName);
}
