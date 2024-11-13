using SettingService.Contracts;

namespace SettingService.Services.Interfaces;

internal interface ICacheService
{
    void Add(SettingItem settingItem);

    SettingItem[] Get(IEnumerable<string> settingNames);

    SettingItem? Get(string settingName);

    void Remove(string settingName);
}
