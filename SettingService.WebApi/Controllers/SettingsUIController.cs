using Microsoft.AspNetCore.Mvc;
using SettingService.Entities;
using SettingService.FrontModels;
using SettingService.Services.Interfaces;

namespace SettingService.WebApi.Controllers;

/// <summary>
/// Контроллер для управлением настройками.
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class SettingsUIController : ControllerBase
{
    private readonly ISettingsService _settingsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SettingsUIController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>
    /// Получить все настройки для приложения.
    /// </summary>
    /// <param name="applicationName">Название приложения</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<OperationResult<SettingFrontModel>> GetAll(string? applicationName = null)
    {
        var result = await _settingsService.GetAll(applicationName);

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Добавить новую настройку.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<OperationResult<SettingItemFrontModel>> Add(SettingItemFrontModel setting)
    {
        var result = await _settingsService.Add(setting);

        return result;
    }

    /// <summary>
    /// Редактировать настройку.
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    public async Task<OperationResult<SettingItemFrontModel>> Edit(SettingItemFrontModel setting)
    {
        var result = await _settingsService.Edit(setting);

        return result;
    }

    /// <summary>
    /// Редактировать приложение.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<OperationResult> Delete(int id)
    {
        var result = await _settingsService.Delete(id);

        return result;
    }
}
