using System.Text.Json.Serialization;

namespace SettingService.Contracts;

/// <summary>
/// Объект, представляющий настройку отправки в приложение
/// </summary>
public class SettingItem
{
    /// <summary>
    /// Название настройки
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Значение настройки
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
    
    /// <summary>
    /// Тип значения настройки
    /// </summary>
    [JsonPropertyName("valueType")]
    public SettingValueTypeEnum ValueType { get; set; }
}
