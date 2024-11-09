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

    public static bool EqualTo(this Setting setting, SettingItemFrontModel frontModel)
    {
        return setting.Name == frontModel.Name &&
            setting.Description == frontModel.Description &&
            setting.ValueType == frontModel.ValueType &&
            setting.Value == frontModel.Value &&
            setting.ExternalSourceType == frontModel.ExternalSourceType &&
            setting.IsFromExternalSource == frontModel.IsFromExternalSource &&
            setting.ExternalSourcePath == frontModel.ExternalSourcePath &&
            setting.ExternalSourceKey == frontModel.ExternalSourceKey &&
            setting.Applications.Select(x => x.Name).SequenceEqual(frontModel.ApplicationNames);
    }
}
