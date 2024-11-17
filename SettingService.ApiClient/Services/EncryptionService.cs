using SettingService.ApiClient.Interfaces;
using System.Security.Cryptography;
using System.Text;

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
        var encryptedBytes = Convert.FromBase64String(encryptedValue);

        byte[] decryptedBytes;

        using (var importedKey = CngKey.Import(_privateKey, CngKeyBlobFormat.Pkcs8PrivateBlob))
        using (var rsa = new RSACng(importedKey))
        {
            decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        }

        var value = Encoding.UTF8.GetString(decryptedBytes);

        return value;
    }
}
