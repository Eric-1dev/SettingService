namespace SettingService.Services.Interfaces;

public interface IEncryptionService
{
    string GetKey();
    string Encrypt(string value);
    string Decrypt(string encryptedValue);
}
