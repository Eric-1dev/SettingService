using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SettingService.DataLayer;
using SettingService.Entities;
using SettingService.FrontModels;
using SettingService.Services.Extensions;
using SettingService.Services.Interfaces.UI;

namespace SettingService.Services.Implementation.UI;

internal class ApplicationsUIService : IApplicationsUIService
{
    private readonly IDbContextFactory<SettingsContext> _dbContextFactory;
    private readonly ILogger _logger;

    public ApplicationsUIService(IDbContextFactory<SettingsContext> dbContextFactory, ILogger<ApplicationsUIService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyCollection<ApplicationFrontModel>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var apps = await dbContext.Applications
                .Select(x => x.MapToFrontModel())
                .ToArrayAsync(cancellationToken);

            return OperationResult<IReadOnlyCollection<ApplicationFrontModel>>.Success(apps);
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось получить список приложений";
            _logger.LogError(ex, errorMessage);
            return OperationResult<IReadOnlyCollection<ApplicationFrontModel>>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult<ApplicationFrontModel>> Add(ApplicationFrontModel app, CancellationToken cancellationToken = default)
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var isAlreadyExists = await dbContext.Applications.AnyAsync(x => x.Name == app.Name, cancellationToken);
            if (isAlreadyExists)
                return OperationResult<ApplicationFrontModel>.Fail("Приложение с таким названием уже существует");

            var application = app.MapFromFrontModel();

            await dbContext.Applications.AddAsync(application, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return OperationResult<ApplicationFrontModel>.Success(application.MapToFrontModel());
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось добавить приложение";
            _logger.LogError(ex, errorMessage);
            return OperationResult<ApplicationFrontModel>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult<ApplicationFrontModel>> Edit(ApplicationFrontModel app, CancellationToken cancellationToken = default)
    {
        if (app.Key == null)
        {
            const string errorMessage = "Не указан ID редактируемого приложения";
            _logger.LogError(errorMessage);
            return OperationResult<ApplicationFrontModel>.Fail(string.Format(errorMessage));
        }

        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var currentApp = await dbContext.Applications.FirstOrDefaultAsync(x => x.Id == app.Key, cancellationToken);

            if (currentApp == null)
            {
                const string errorMessageTemplate = "Приложение с ID = {0} не найдено в базе";
                _logger.LogError(errorMessageTemplate, app.Key);
                return OperationResult<ApplicationFrontModel>.Fail(string.Format(errorMessageTemplate, app.Key));
            }

            if (currentApp.EqualTo(app))
                return OperationResult<ApplicationFrontModel>.Fail("Не обнаружено изменений в параметрах приложения");

            currentApp.Name = app.Name;
            currentApp.Description = app.Description;

            await dbContext.SaveChangesAsync(cancellationToken);

            return OperationResult<ApplicationFrontModel>.Success(currentApp.MapToFrontModel());
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось отредактировать приложение";
            _logger.LogError(ex, errorMessage);
            return OperationResult<ApplicationFrontModel>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var currentApp = await dbContext.Applications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (currentApp == null)
            {
                const string errorMessageTemplate = "Приложение с ID = {0} не найдено в базе";
                _logger.LogError(errorMessageTemplate, id);
                return OperationResult.Fail(string.Format(errorMessageTemplate, id));
            }

            dbContext.Applications.Remove(currentApp);

            await dbContext.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось удалить приложение";
            _logger.LogError(ex, errorMessage);
            return OperationResult.Fail(errorMessage);
        }
    }
}
