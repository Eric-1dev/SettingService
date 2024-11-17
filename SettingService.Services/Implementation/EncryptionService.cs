using Microsoft.Extensions.Options;
using SettingService.Services.Interfaces;
using SettingService.Services.Models;
using System.Security.Cryptography;
using System.Text;

namespace SettingService.Services.Implementation;

internal class EncryptionService : IEncryptionService
{
    private static byte[]? _privateKey;
    private static byte[]? _publicKey;
    private static bool _isInitialized = false;
    private static readonly object _locker = new();

    public EncryptionService(IOptions<CryptoSettings> cryptoSettings)
    {
        _privateKey = Convert.FromBase64String(cryptoSettings.Value.PrivateKey);
        _publicKey = Convert.FromBase64String(cryptoSettings.Value.PublicKey);
    }

    public string GetKey()
    {
        var privateKey = _privateKey!;

        return Convert.ToBase64String(privateKey);
    }

    public string Encrypt(string value)
    {
        var decryptedBytes = Encoding.UTF8.GetBytes(value);

        byte[] encryptedBytes;

        using (var rsa = RSA.Create())
        {
            rsa.ImportRSAPublicKey(_publicKey, out int _);
            encryptedBytes = rsa.Encrypt(decryptedBytes, RSAEncryptionPadding.OaepSHA256);
        }

        var encryptedValue = Convert.ToBase64String(encryptedBytes);

        return encryptedValue;
    }

    public string Decrypt(string encryptedValue)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedValue);

        byte[] decryptedBytes;

        using (var rsa = RSA.Create())
        {
            rsa.ImportPkcs8PrivateKey(_privateKey, out int _);
            decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        }

        var value = Encoding.UTF8.GetString(decryptedBytes);

        return value;
    }
}
