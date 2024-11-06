using SettingService.Entities;
using SettingService.Services.Interfaces.ExternalSource;

namespace SettingService.Services.Implementation.ExternalSource;

internal class VaultExternalSourceService : IExternalSourceService
{
    public ExternalSourceTypeEnum ExternalSourceType => ExternalSourceTypeEnum.Vault;

    public string? GetSettingValue(string path, string? key)
    {
        return $"{path}-{key}-value";
    }
}
