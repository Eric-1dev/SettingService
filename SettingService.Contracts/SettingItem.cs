﻿namespace SettingService.Contracts;

/// <summary>
/// Объект, представляющий настройку
/// </summary>
public class SettingItem
{
    public SettingItem(string name, SettingTypeEnum type)
    {
        Name = name;
        Type = type;
    }

    public SettingItem(string name, SettingTypeEnum type, string value) : this(name, type)
    {
        Value = value;
    }

    public SettingItem(string name, SettingTypeEnum type, string value, string description) : this(name, type, value)
    {
        Description = description;
    }

    /// <summary>
    /// Название настройки
    /// </summary>
    public string Name { get; set; }

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
    public SettingTypeEnum Type { get; set; }
}
