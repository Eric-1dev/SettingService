using SettingService.Entities;
using SettingService.Services.Models;

namespace SettingService.Services.Interfaces;

public interface IApplicationsService
{
    Task<OperationResult<IReadOnlyCollection<ApplicationForList>>> GetAllApplications();

    Task<OperationResult> AddApplication(ApplicationForList app);
}