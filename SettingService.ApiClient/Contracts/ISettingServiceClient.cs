namespace SettingService.ApiClient.Contracts;

public interface ISettingServiceClient
{
    Task Start(CancellationToken cancellationToken = default);

    string? GetStringSetting(string settingName);

    int? GetIntSetting(string settingName);

    long? GetLongSetting(string settingName);

    double? GetDoubleSetting(string settingName);

    decimal? GetDecimalSetting(string settingName);

    bool? GetBoolSetting(string settingName);
}
