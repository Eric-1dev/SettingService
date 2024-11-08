using SettingService.ApiClient.Contracts;
using SettingService.Contracts;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace SettingService.ApiClient;

internal sealed class WebApiClient : IWebApiClient
{
    const string GetAllUrl = "api/Setting/GetAll";
    const string GetByNameUrl = "api/Setting/GetByName";

    private readonly HttpClient _client;

    public WebApiClient(HttpClient client, ISettingServiceConfiguration configuration)
    {
        _client = client;
        _client.BaseAddress = new Uri(configuration.SettingServiceUrl);
    }

    public async Task<IReadOnlyCollection<SettingItem>> GetAll(string applicationName, CancellationToken cancellationToken = default)
    {
        var token = GetToken();

        var appNameParam = HttpUtility.UrlEncode(applicationName);
        var requestUrl = $"{GetAllUrl}/?applicationName={appNameParam}";

        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync(requestUrl, cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var settings = JsonSerializer.Deserialize<IReadOnlyCollection<SettingItem>>(content);

        return settings;
    }

    public async Task<SettingItem> GetByName(string applicationName, string settingName, CancellationToken cancellationToken = default)
    {
        var token = GetToken();

        var appNameParam = HttpUtility.UrlEncode(applicationName);
        var settingNameParam = HttpUtility.UrlEncode(settingName);

        var requestUrl = HttpUtility.UrlEncode($"{GetByNameUrl}/?applicationName={appNameParam}&settingName={settingNameParam}");

        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync(requestUrl, cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var settings = JsonSerializer.Deserialize<SettingItem>(content);

        return settings;
    }

    private string GetToken()
    {
        return "TokenString";
    }
}
