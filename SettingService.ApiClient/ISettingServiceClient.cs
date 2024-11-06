using SettingService.Contracts;

namespace SettingService.ApiClient;

public interface ISettingServiceClient
{
    Task<IReadOnlyCollection<SettingItem>> GetAllSettings(string applicationName, CancellationToken cancellationToken = default);
}
