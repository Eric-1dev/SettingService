using SettingService.Contracts;
using SettingService.Services.Interfaces;
using System.Collections.Concurrent;

namespace SettingService.Services.Implementation;

internal class CacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, SettingItem> _settingsCache = [];

    public void Add(SettingItem settingItem)
    {
        _settingsCache.AddOrUpdate(settingItem.Name!, settingItem, (settingName, oldValue) => settingItem);
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

    public void Remove(string settingName)
    {
        _settingsCache.TryRemove(settingName, out var _);
    }
}
