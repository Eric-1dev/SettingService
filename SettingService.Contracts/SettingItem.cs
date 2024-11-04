namespace SettingService.Contracts;

/// <summary>
/// Объект, представляющий настройку отправки в приложение
/// </summary>
public class SettingItem
{
    //public SettingItem(string name, SettingValueTypeEnum type)
    //{
    //    Name = name;
    //    Type = type;
    //}

    //public SettingItem(string name, SettingValueTypeEnum type, string value) : this(name, type)
    //{
    //    Value = value;
    //}

    //public SettingItem(string name, SettingValueTypeEnum type, string value, string description) : this(name, type, value)
    //{
    //    Description = description;
    //}

    public int Id { get; set; }

    /// <summary>
    /// Название настройки
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Описание настройки
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Значение настройки
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// Тип значения настройки
    /// </summary>
    public SettingValueTypeEnum ValueType { get; set; }
}
