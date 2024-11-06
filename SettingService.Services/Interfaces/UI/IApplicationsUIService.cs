using SettingService.Entities;
using SettingService.FrontModels;

namespace SettingService.Services.Interfaces.UI;

public interface IApplicationsUIService
{
    Task<OperationResult<IReadOnlyCollection<ApplicationFrontModel>>> GetAll(CancellationToken cancellationToken = default);

    Task<OperationResult<ApplicationFrontModel>> Add(ApplicationFrontModel app, CancellationToken cancellationToken = default);

    Task<OperationResult<ApplicationFrontModel>> Edit(ApplicationFrontModel app, CancellationToken cancellationToken = default);

    Task<OperationResult> Delete(int id, CancellationToken cancellationToken = default);
}