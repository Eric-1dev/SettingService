using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SettingService.DataLayer;
using SettingService.DataModel;
using SettingService.Entities;
using SettingService.Services.Interfaces;
using SettingService.Services.Models;

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

    public async Task<OperationResult<IReadOnlyCollection<ApplicationForList>>> GetAllApplications()
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var apps = await dbContext.Applications
                .Select(x => new ApplicationForList { Name = x.Name })
                .ToArrayAsync();

            return OperationResult<IReadOnlyCollection<ApplicationForList>>.Success(apps);
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось получить список приложений";
            _logger.LogError(ex, errorMessage);
            return OperationResult<IReadOnlyCollection<ApplicationForList>>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult> AddApplication(ApplicationForList app)
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            await dbContext.Applications.AddAsync(new Application { Name = app.Name });

            await dbContext.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось добавить приложение";
            _logger.LogError(ex, errorMessage);
            return OperationResult.Fail(errorMessage);
        }
    }
}
