namespace SettingService.ApiClient.Interfaces;

internal interface IEncryptionService
{
    void Initialize(string privateKey);
    string Decrypt(string encryptedValue);
}
