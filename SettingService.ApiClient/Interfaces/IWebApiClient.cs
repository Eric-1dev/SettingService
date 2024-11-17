using SettingService.Contracts;

namespace SettingService.ApiClient.Interfaces;

internal interface IWebApiClient
{
    Task<IReadOnlyCollection<SettingItem>> GetAll(string applicationName, CancellationToken cancellationToken = default);

    Task<SettingItem?> GetByName(string applicationName, string settingName, CancellationToken cancellationToken = default);
    
    Task<string> GetPrivateKey(CancellationToken cancellationToken = default);
}
