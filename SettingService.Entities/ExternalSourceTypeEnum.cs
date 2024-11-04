using System.Text.Json.Serialization;

namespace SettingService.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExternalSourceTypeEnum
{
    Vault = 1
}
