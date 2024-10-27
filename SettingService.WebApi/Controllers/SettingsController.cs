using Microsoft.AspNetCore.Mvc;
using SettingService.Contracts;

namespace SettingService.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class SettingsController : ControllerBase
{
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
            new("Setting1", SettingTypeEnum.String, "lalala"),
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
        var setting = new SettingItem("Setting1", SettingTypeEnum.String, "lalala");

        return await Task.FromResult(Ok(setting));
    }
}
