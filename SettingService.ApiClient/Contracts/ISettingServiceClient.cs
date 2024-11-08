using SettingService.Contracts;

namespace SettingService.ApiClient.Contracts;

public interface ISettingServiceClient
{
    Task<IReadOnlyCollection<SettingItem>> Start(CancellationToken cancellationToken);
}
