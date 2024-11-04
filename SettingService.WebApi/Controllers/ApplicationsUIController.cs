using Microsoft.AspNetCore.Mvc;
using SettingService.Entities;
using SettingService.FrontModels;
using SettingService.Services.Interfaces;

namespace SettingService.WebApi.Controllers;

/// <summary>
/// Контроллер для управлением приложениями.
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class ApplicationsUIController : ControllerBase
{
    private readonly IApplicationsService _applicationsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ApplicationsUIController(IApplicationsService applicationsService)
    {
        _applicationsService = applicationsService;
    }

    /// <summary>
    /// Получить список всех приложений для страницы управления приложениями.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<OperationResult<IReadOnlyCollection<ApplicationFrontModel>>> GetAll()
    {
        var result = await _applicationsService.GetAll();

        return result;
    }

    /// <summary>
    /// Добавить новое приложение.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<OperationResult<ApplicationFrontModel>> Add(ApplicationFrontModel app)
    {
        var result = await _applicationsService.Add(app);

        return result;
    }

    /// <summary>
    /// Редактировать приложение.
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    public async Task<OperationResult<ApplicationFrontModel>> Edit(ApplicationFrontModel app)
    {
        var result = await _applicationsService.Edit(app);

        return result;
    }

    /// <summary>
    /// Редактировать приложение.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<OperationResult> Delete(int id)
    {
        var result = await _applicationsService.Delete(id);

        return result;
    }
}