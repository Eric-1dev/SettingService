using SettingService.Entities;
using SettingService.FrontModels;

namespace SettingService.Services.Interfaces;

public interface ISettingsService
{
    Task<OperationResult<SettingFrontModel>> GetAll(string? applicationName = null);

    Task<OperationResult<SettingItemFrontModel>> Add(SettingItemFrontModel setting);

    Task<OperationResult<SettingItemFrontModel>> Edit(SettingItemFrontModel setting);

    Task<OperationResult> Delete(int id);
}