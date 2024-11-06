using Microsoft.AspNetCore.Mvc;
using SettingService.Entities;
using SettingService.FrontModels;
using SettingService.Services.Interfaces.UI;

namespace SettingService.WebApi.Controllers.UI;

/// <summary>
/// Контроллер для управлением приложениями.
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class ApplicationsUIController : ControllerBase
{
    private readonly IApplicationsUIService _applicationsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ApplicationsUIController(IApplicationsUIService applicationsService)
    {
        _applicationsService = applicationsService;
    }

    /// <summary>
    /// Получить список всех приложений для страницы управления приложениями.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<OperationResult<IReadOnlyCollection<ApplicationFrontModel>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _applicationsService.GetAll(cancellationToken);

        return result;
    }

    /// <summary>
    /// Добавить новое приложение.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<OperationResult<ApplicationFrontModel>> Add(ApplicationFrontModel app, CancellationToken cancellationToken)
    {
        var result = await _applicationsService.Add(app, cancellationToken);

        return result;
    }

    /// <summary>
    /// Редактировать приложение.
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    public async Task<OperationResult<ApplicationFrontModel>> Edit(ApplicationFrontModel app, CancellationToken cancellationToken)
    {
        var result = await _applicationsService.Edit(app, cancellationToken);

        return result;
    }

    /// <summary>
    /// Редактировать приложение.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<OperationResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _applicationsService.Delete(id, cancellationToken);

        return result;
    }
}