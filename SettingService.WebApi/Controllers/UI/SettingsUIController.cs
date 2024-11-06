using Microsoft.AspNetCore.Mvc;
using SettingService.Entities;
using SettingService.FrontModels;
using SettingService.Services.Interfaces.UI;

namespace SettingService.WebApi.Controllers.UI;

/// <summary>
/// Контроллер для управлением настройками.
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class SettingsUIController : ControllerBase
{
    private readonly ISettingsUIService _settingsUIService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SettingsUIController(ISettingsUIService settingsUIService)
    {
        _settingsUIService = settingsUIService;
    }

    /// <summary>
    /// Получить все настройки для приложения.
    /// </summary>
    /// <param name="applicationName">Название приложения</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<OperationResult<SettingFrontModel>> GetAll(CancellationToken cancellationToken, string? applicationName = null)
    {
        var result = await _settingsUIService.GetAll(applicationName, cancellationToken);

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Добавить новую настройку.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<OperationResult<SettingItemFrontModel>> Add(SettingItemFrontModel setting, CancellationToken cancellationToken)
    {
        var result = await _settingsUIService.Add(setting, cancellationToken);

        return result;
    }

    /// <summary>
    /// Редактировать настройку.
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    public async Task<OperationResult<SettingItemFrontModel>> Edit(SettingItemFrontModel setting, CancellationToken cancellationToken)
    {
        var result = await _settingsUIService.Edit(setting, cancellationToken);

        return result;
    }

    /// <summary>
    /// Редактировать приложение.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<OperationResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _settingsUIService.Delete(id, cancellationToken);

        return result;
    }
}
