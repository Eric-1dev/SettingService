using SettingService.Entities;

namespace SettingService.Services.Interfaces.ExternalSource;

internal interface IExternalSourceService
{
    ExternalSourceTypeEnum ExternalSourceType { get; }

    Task<string?> GetSettingValueAsync(string path, string? key);
}
