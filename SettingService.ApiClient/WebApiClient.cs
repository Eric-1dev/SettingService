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

    public WebApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyCollection<SettingItem>> GetAll(string applicationName, CancellationToken cancellationToken = default)
    {
        var token = GetToken();

        var requestUrl = HttpUtility.UrlEncode($"{GetAllUrl}/?applicationName={applicationName}");

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

        var requestUrl = HttpUtility.UrlEncode($"{GetByNameUrl}/?applicationName={applicationName}&settingName={settingName}");

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
