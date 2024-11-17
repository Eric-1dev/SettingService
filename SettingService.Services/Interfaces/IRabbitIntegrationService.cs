using SettingService.Contracts;

namespace SettingService.Services.Interfaces;

public interface IRabbitIntegrationService
{
    Task Initialize();
    
    Task PublishChange(SettingItem settingItem, string[] applicationNames, SettingChangeTypeEnum changeType, string? oldName);
}
