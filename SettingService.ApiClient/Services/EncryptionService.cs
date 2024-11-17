using SettingService.ApiClient.Interfaces;

namespace SettingService.ApiClient.Services;

internal class EncryptionService : IEncryptionService
{
    private byte[]? _privateKey;

    public void Initialize(string privateKey)
    {
        _privateKey = Convert.FromBase64String(privateKey);
    }

    public string Decrypt(string encryptedValue)
    {
        return "";
    }
}
