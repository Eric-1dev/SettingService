using SettingService.Entities;
using SettingService.FrontModels;

namespace SettingService.Services.Interfaces.UI;

public interface ISettingsUIService
{
    Task<OperationResult<SettingFrontModel>> GetAll(string? applicationName = null, CancellationToken cancellationToken = default);

    Task<OperationResult<SettingItemFrontModel>> Add(SettingItemFrontModel setting, CancellationToken cancellationToken = default);

    Task<OperationResult<SettingItemFrontModel>> Edit(SettingItemFrontModel setting, CancellationToken cancellationToken = default);

    Task<OperationResult> Delete(int id, CancellationToken cancellationToken = default);
}