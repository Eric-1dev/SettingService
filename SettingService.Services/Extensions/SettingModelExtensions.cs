using SettingService.DataModel;
using SettingService.FrontModels;

namespace SettingService.Services.Extensions;

public static class SettingModelExtensions
{
    public static SettingItemFrontModel MapToFrontModel(this Setting setting)
    {
        return new SettingItemFrontModel
        {
            Key = setting.Id,
            Name = setting.Name,
            Description = setting.Description,
            ValueType = setting.ValueType,
            IsFromExternalSource = setting.IsFromExternalSource,
            Value = setting.Value,
            ExternalSourceType = setting.ExternalSourceType,
            ExternalSourcePath = setting.ExternalSourcePath,
            ExternalSourceKey = setting.ExternalSourceKey,
            ApplicationNames = setting.Applications.Select(x => x.Name).ToArray()
        };
    }
}
