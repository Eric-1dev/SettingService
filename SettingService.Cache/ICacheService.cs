using SettingService.Contracts;

namespace SettingService.Cache;

public interface ICacheService
{
    void Initialize(IReadOnlyCollection<SettingItem> settingItems);

    void HandleUpdate(SettingChangeTypeEnum changeType, SettingItem settingItem, string? oldSettingName);

    SettingItem[] Get(IEnumerable<string> settingNames);

    SettingItem? Get(string settingName);
}
