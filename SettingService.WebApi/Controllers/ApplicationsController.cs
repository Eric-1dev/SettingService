using Microsoft.AspNetCore.Mvc;
using SettingService.Contracts;
using SettingService.Services.Interfaces;
using SettingService.Services.Models;

namespace SettingService.WebApi.Controllers;

/// <summary>
/// Контроллер для управлением приложениями.
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationsService _applicationsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ApplicationsController(IApplicationsService applicationsService)
    {
        _applicationsService = applicationsService;
    }

    /// <summary>
    /// Получить список всех приложений для страницы управления приложениями.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType<ICollection<ApplicationForList>>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllApplications()
    {
        var result = await _applicationsService.GetAllApplications();

        if (result.IsSuccess)
            return Ok(result.Entity);

        return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
    }

    /// <summary>
    /// Добавить новое приложение.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddApplication(ApplicationForList app)
    {
        var result = await _applicationsService.AddApplication(new ApplicationForList { Name = app.Name, Description = app.Description });

        if (result.IsSuccess)
            return Ok();

        return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
    }
}
