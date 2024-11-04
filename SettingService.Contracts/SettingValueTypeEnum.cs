using System.Text.Json.Serialization;

namespace SettingService.Contracts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SettingValueTypeEnum
{
    Int = 1,
    Long = 2,
    Double = 3,
    Decimal = 4,
    String = 5,
    Bool = 6
}
