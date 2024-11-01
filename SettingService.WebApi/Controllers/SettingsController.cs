﻿using Microsoft.AspNetCore.Mvc;
using SettingService.Contracts;
using SettingService.Services.Interfaces;

namespace SettingService.WebApi.Controllers;

/// <summary>
/// Контроллер для управлением настройками
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _settingsService;

    /// <summary>
    /// Конструктор
    /// </summary>
    public SettingsController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>
    /// Получить все настройки для приложения
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType<ICollection<SettingItem>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSettings(string application)
    {
        var settings = new SettingItem[]
        {
            new("Setting1", SettingTypeEnum.String, "lalala", "что-то непонятное"),
            new("Setting2", SettingTypeEnum.Int, "1")
        };

        return await Task.FromResult(Ok(settings));
    }

    /// <summary>
    /// Получить указанную настройку для приложения
    /// </summary>
    /// <param name="applcationName"></param>
    /// <param name="settingName"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType<SettingItem>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSettingByName(string applcationName, string settingName)
    {
        var setting = new SettingItem("Setting1", SettingTypeEnum.String, "lalala", "что-то непонятное");

        return await Task.FromResult(Ok(setting));
    }
}
