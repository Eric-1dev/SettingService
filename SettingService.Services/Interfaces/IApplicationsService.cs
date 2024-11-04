using SettingService.Entities;
using SettingService.FrontModels;

namespace SettingService.Services.Interfaces;

public interface IApplicationsService
{
    Task<OperationResult<IReadOnlyCollection<ApplicationFrontModel>>> GetAll();

    Task<OperationResult<ApplicationFrontModel>> Add(ApplicationFrontModel app);

    Task<OperationResult<ApplicationFrontModel>> Edit(ApplicationFrontModel app);

    Task<OperationResult> Delete(int id);
}