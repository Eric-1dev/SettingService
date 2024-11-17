namespace SettingService.Contracts;

public class RabbitMessage
{
    public SettingChangeTypeEnum ChangeType { get; set; }
    
    public string? OldSettingName { get; set; }

    public string? CurrentName { get; set; }

    public string? EncryptedValue { get; set; }

    public SettingValueTypeEnum ValueType { get; set; }
}
