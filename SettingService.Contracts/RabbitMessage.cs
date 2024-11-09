namespace SettingService.Contracts;

public class RabbitMessage
{
    public SettingChangeTypeEnum ChangeTypeEnum { get; set; }
    public string SettingName { get; set; }
    public string? OldSettingName { get; set; }
}
