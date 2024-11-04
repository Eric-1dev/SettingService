namespace SettingService.FrontModels;

/// <summary>
/// Модель приложения для вывода на UI.
/// </summary>
public class ApplicationFrontModel
{
    /// <summary>
    /// ID приложения.
    /// </summary>
    public int? Key { get; init; }

    /// <summary>
    /// Название приложения.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Опимание приложения.
    /// </summary>
    public string? Description { get; init; }
}
