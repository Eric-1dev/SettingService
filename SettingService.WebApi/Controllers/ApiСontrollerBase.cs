using Microsoft.AspNetCore.Mvc;

namespace SettingService.WebApi.Controllers;

/// <summary>
/// Базовый API-контроллер
/// </summary>
public abstract class ApiСontrollerBase : ControllerBase
{
    /// <summary>
    /// Успешный ответ
    /// </summary>
    /// <typeparam name="TEntity">Тип объекта</typeparam>
    /// <param name="entity">Возвращаемый объект</param>
    /// <returns></returns>
    [NonAction]
    protected JsonResult Success<TEntity>(TEntity entity)
    {
        return new JsonResult(new { IsSuccess = true, Entity = entity });
    }

    /// <summary>
    /// Успешный ответ
    /// </summary>
    /// <returns></returns>
    [NonAction]
    protected JsonResult Success()
    {
        return new JsonResult(new { IsSuccess = true });
    }

    /// <summary>
    /// Провальный ответ
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <returns></returns>
    [NonAction]
    protected JsonResult Fail(string? message)
    {
        return new JsonResult(new { IsSuccess = false, Message = message });
    }
}
