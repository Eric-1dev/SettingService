using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SettingService.DataLayer;
using SettingService.Entities;
using SettingService.FrontModels;
using SettingService.Services.Extensions;
using SettingService.Services.Interfaces;

namespace SettingService.Services.Implementation;

internal class ApplicationsService : IApplicationsService
{
    private readonly IDbContextFactory<SettingsContext> _dbContextFactory;
    private readonly ILogger _logger;

    public ApplicationsService(IDbContextFactory<SettingsContext> dbContextFactory, ILogger<ApplicationsService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyCollection<ApplicationFrontModel>>> GetAll()
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var apps = await dbContext.Applications
                .Select(x => x.MapToFrontModel())
                .ToArrayAsync();

            return OperationResult<IReadOnlyCollection<ApplicationFrontModel>>.Success(apps);
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось получить список приложений";
            _logger.LogError(ex, errorMessage);
            return OperationResult<IReadOnlyCollection<ApplicationFrontModel>>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult<ApplicationFrontModel>> Add(ApplicationFrontModel app)
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var application = app.MapFromFrontModel();

            await dbContext.Applications.AddAsync(application);

            await dbContext.SaveChangesAsync();

            return OperationResult<ApplicationFrontModel>.Success(application.MapToFrontModel());
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось добавить приложение";
            _logger.LogError(ex, errorMessage);
            return OperationResult<ApplicationFrontModel>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult<ApplicationFrontModel>> Edit(ApplicationFrontModel app)
    {
        if (app.Key == null)
        {
            const string errorMessage = "Не указан ID редактируемого приложения";
            _logger.LogError(errorMessage);
            return OperationResult<ApplicationFrontModel>.Fail(string.Format(errorMessage));
        }

        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var currentApp = await dbContext.Applications.FirstOrDefaultAsync(x => x.Id == app.Key);

            if (currentApp == null)
            {
                const string errorMessageTemplate = "Приложение с ID = {0} не найдено в базе";
                _logger.LogError(errorMessageTemplate, app.Key);
                return OperationResult<ApplicationFrontModel>.Fail(string.Format(errorMessageTemplate, app.Key));
            }

            currentApp.Name = app.Name;
            currentApp.Description = app.Description;

            await dbContext.SaveChangesAsync();

            return OperationResult<ApplicationFrontModel>.Success(currentApp.MapToFrontModel());
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось отредактировать приложение";
            _logger.LogError(ex, errorMessage);
            return OperationResult<ApplicationFrontModel>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult> Delete(int id)
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var currentApp = await dbContext.Applications.FirstOrDefaultAsync(x => x.Id == id);

            if (currentApp == null)
            {
                const string errorMessageTemplate = "Приложение с ID = {0} не найдено в базе";
                _logger.LogError(errorMessageTemplate, id);
                return OperationResult.Fail(string.Format(errorMessageTemplate, id));
            }

            dbContext.Applications.Remove(currentApp);

            await dbContext.SaveChangesAsync();

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
