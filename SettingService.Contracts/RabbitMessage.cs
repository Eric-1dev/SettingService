namespace SettingService.Contracts;

public class RabbitMessage
{
    public SettingChangeTypeEnum ChangeTypeEnum { get; set; }
    
    public string? OldSettingName { get; set; }

    public SettingItem SettingItem { get; set; }
}
