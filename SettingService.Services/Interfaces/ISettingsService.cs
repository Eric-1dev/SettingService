using SettingService.Contracts;
using SettingService.DataModel;
using SettingService.Entities;

namespace SettingService.Services.Interfaces;

public interface ISettingsService
{
    Task<OperationResult<IReadOnlyCollection<SettingItem>>> GetAll(string applicationName, CancellationToken cancellationToken);
    
    Task<OperationResult<SettingItem>> GetByName(string applicationName, string settingName, CancellationToken cancellationToken);

    Task<SettingItem> GetSettingItemFromDao(Setting setting);
    
    Task HandleRabbitMessage(RabbitMessage message);
}
