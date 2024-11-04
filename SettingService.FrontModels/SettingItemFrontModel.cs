using SettingService.Contracts;
using SettingService.Entities;

namespace SettingService.FrontModels;

/// <summary>
/// Модель настройки для UI.
/// </summary>
public class SettingItemFrontModel
{
    /// <summary>
    /// ID настройки.
    /// </summary>
    public int? Key { get; init; }

    /// <summary>
    /// Название настройки.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Описание настройки.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Значение настройки.
    /// </summary>
    public string? Value { get; init; }

    /// <summary>
    /// Тип значения настройки.
    /// </summary>
    public SettingValueTypeEnum ValueType { get; init; }

    /// <summary>
    /// Признак, обозначающий, что значение получено из внешнего источника.
    /// </summary>
    public bool IsFromExternalSource { get; init; }

    /// <summary>
    /// Тип внешнего источника хранения настроек.
    /// </summary>
    public ExternalSourceTypeEnum? ExternalSourceType { get; init; }

    /// <summary>
    /// Путь до значения настройки во внешнем источнике.
    /// </summary>
    public string? ExternalSourcePath { get; init; }

    /// <summary>
    /// Ключ значения настройки во внешнем источнике.
    /// </summary>
    public string? ExternalSourceKey { get; init; }

    /// <summary>
    /// Список приложений, к которым привязана настройка.
    /// </summary>
    public IReadOnlyCollection<string> ApplicationNames { get; set; } = [];
}
