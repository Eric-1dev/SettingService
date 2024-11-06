using SettingService.Contracts;
using System.Text.Json;

namespace SettingService.ApiClient;

public sealed class SettingServiceClient : ISettingServiceClient
{
    const string GetAllUrl = "api/Setting/GetAll";
    const string GetByNameUrl = "api/Setting/GetByName";

    private readonly HttpClient _client;

    public SettingServiceClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyCollection<SettingItem>> GetAllSettings(string applicationName, CancellationToken cancellationToken = default)
    {
        var token = GetToken();

        var response = await _client.GetAsync($"{GetAllUrl}/?applicationName={applicationName}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var settings = JsonSerializer.Deserialize<IReadOnlyCollection<SettingItem>>(content);

        return settings;
    }

    private string GetToken()
    {
        return "TokenString";
    }
}
