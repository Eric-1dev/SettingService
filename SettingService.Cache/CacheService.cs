using SettingService.Contracts;
using System.Collections.Concurrent;

namespace SettingService.Cache;

internal class CacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, SettingItem> _settingsCache = [];

    public void Initialize(IReadOnlyCollection<SettingItem> settingItems)
    {
        _settingsCache.Clear();

        foreach (var settingItem in settingItems)
        {
            _settingsCache.AddOrUpdate(settingItem.Name!, settingItem, (settingName, oldValue) => settingItem);
        }
    }

    public SettingItem[] Get(IEnumerable<string> settingNames)
    {
        var settingItems = _settingsCache.Values.Where(x => settingNames.Contains(x.Name));
        return [.. settingItems];
    }

    public SettingItem? Get(string settingName)
    {
        _settingsCache.TryGetValue(settingName, out var settingItem);

        return settingItem;
    }

    public void HandleUpdate(SettingChangeTypeEnum changeType, SettingItem settingItem, string? oldSettingName)
    {
        switch (changeType)
        {
            case SettingChangeTypeEnum.Added:
                _settingsCache.AddOrUpdate(settingItem.Name!, settingItem, (settingName, oldValue) => settingItem);
                break;
            case SettingChangeTypeEnum.Removed:
                _settingsCache.TryRemove(settingItem.Name!, out var _);
                break;
            case SettingChangeTypeEnum.Changed:
                if (oldSettingName != null && oldSettingName != settingItem.Name)
                {
                    _settingsCache.TryRemove(oldSettingName, out var _);
                }

                _settingsCache.AddOrUpdate(settingItem.Name!, settingItem, (settingName, oldValue) => settingItem);
                break;
        }
    }
}
