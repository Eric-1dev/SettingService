using SettingService.Entities;

namespace SettingService.Services.Interfaces.ExternalSource;

internal interface IExternalSourceService
{
    ExternalSourceTypeEnum ExternalSourceType { get; }

    string? GetSettingValue(string? path, string? key);
}
