using Microsoft.AspNetCore.Mvc;
using SettingService.Contracts;
using SettingService.Entities;
using SettingService.FrontModels;

namespace SettingService.WebApi.Controllers;

/// <summary>
/// Контроллер для управлением настройками.
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class SettingBackendController : ControllerBase
{
    /// <summary>
    /// Получить указанную настройку для приложения.
    /// </summary>
    /// <param name="applcationName">Название приложения</param>
    /// <param name="settingName">Название настройки</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<OperationResult<SettingItemFrontModel>> GetByName(string applcationName, string settingName)
    {
        var setting = new SettingItemFrontModel()
        {
            Key = 1,
            Name = "Setting2",
            Description = "aag",
            ValueType = SettingValueTypeEnum.String,
            Value = "что-то непонятное",
            ApplicationNames = ["sdfsdf", "CCS22"]
        };

        var result = OperationResult<SettingItemFrontModel>.Success(setting);

        return await Task.FromResult(result);
    }
}
