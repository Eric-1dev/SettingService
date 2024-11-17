using Microsoft.AspNetCore.Mvc;
using SettingService.Contracts;
using SettingService.Services.Interfaces;

namespace SettingService.WebApi.Controllers;

/// <summary>
/// Контроллер для управлением настройками.
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class SettingController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly IEncryptionService _encryptionService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="settingsService"></param>
    /// <param name="encryptionService"></param>
    public SettingController(ISettingsService settingsService, IEncryptionService encryptionService)
    {
        _settingsService = settingsService;
        _encryptionService = encryptionService;
    }

    /// <summary>
    /// Получить указанную настройку для приложения.
    /// </summary>
    /// <param name="applicationName">Название приложения</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IReadOnlyCollection<SettingItem>> GetAll(string applicationName, CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetAll(applicationName, cancellationToken);

        if (result.IsSuccess)
            return result.Entity!;

        throw new Exception($"Ошибка в работе приложения SettingService. Error: {result.Message}");
    }

    /// <summary>
    /// Получить указанную настройку для приложения.
    /// </summary>
    /// <param name="applicationName">Название приложения</param>
    /// <param name="settingName">Название настройки</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SettingItem> GetByName(string applicationName, string settingName, CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetByName(applicationName, settingName, cancellationToken);

        if (result.IsSuccess)
            return result.Entity!;

        throw new Exception($"Ошибка в работе приложения SettingService. Error: {result.Message}");
    }

    /// <summary>
    /// Получения приватного ключа для дешифровки значения настройки, полученной через RabbitMQ
    /// </summary>
    /// <returns></returns>
    public string GetKey()
    {
        return _encryptionService.GetKey();
    }
}
