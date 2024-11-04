using SettingService.Contracts;

namespace SettingService.ApiClient;

public sealed class SettingServiceClient : ISettingServiceClient
{
    public SettingServiceClient(IHttpClientFactory httpClientFactory, string baseUrl)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);
    }

    private TSetting GetSetting<TSetting>(string settingName) where TSetting : SettingItem, new()
    {
        //return new TSetting(settingName, SettingTypeEnum.Long, "value1");
        return new TSetting();
    }
}
