using Microsoft.AspNetCore.Mvc;
using SettingService.Services.Interfaces;
using SettingService.Services.Models;

namespace SettingService.WebApi.Controllers;

/// <summary>
/// Контроллер для управлением приложениями.
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class ApplicationsController : ApiСontrollerBase
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
    public async Task<JsonResult> GetAllApplications()
    {
        var result = await _applicationsService.GetAllApplications();

        if (result.IsSuccess)
            return Success(result.Entity);

        return Fail(result.Message);
    }

    /// <summary>
    /// Добавить новое приложение.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> AddApplication(string name)
    {
        var result = await _applicationsService.AddApplication(new ApplicationForList { Name = name });

        if (result.IsSuccess)
            return Success();

        return Fail(result.Message);
    }
}
