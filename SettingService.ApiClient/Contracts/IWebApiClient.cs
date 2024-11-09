using SettingService.Contracts;

namespace SettingService.ApiClient.Contracts;

internal interface IWebApiClient
{
    Task<IReadOnlyCollection<SettingItem>> GetAll(string applicationName, CancellationToken cancellationToken = default);

    Task<SettingItem?> GetByName(string applicationName, string settingName, CancellationToken cancellationToken = default);
}
