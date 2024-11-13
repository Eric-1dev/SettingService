using SettingService.Entities;
using SettingService.Services.Interfaces.ExternalSource;

namespace SettingService.Services.Implementation.ExternalSource;

internal class VaultExternalSourceService : IExternalSourceService
{
    public ExternalSourceTypeEnum ExternalSourceType => ExternalSourceTypeEnum.Vault;

    public async Task<string?> GetSettingValueAsync(string path, string? key)
    {
        return await Task.FromResult($"{path}-{key}-value");
    }
}
