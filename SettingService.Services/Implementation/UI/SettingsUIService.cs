using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SettingService.DataLayer;
using SettingService.DataModel;
using SettingService.Entities;
using SettingService.FrontModels;
using SettingService.Services.Extensions;
using SettingService.Services.Interfaces.UI;

namespace SettingService.Services.Implementation.UI;

internal class SettingsUIService : ISettingsUIService
{
    private readonly IDbContextFactory<SettingsContext> _dbContextFactory;
    private readonly ILogger _logger;

    public SettingsUIService(IDbContextFactory<SettingsContext> dbContextFactory, ILogger<SettingsUIService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<OperationResult<SettingFrontModel>> GetAll(string? applicationName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var apps = await dbContext.Applications.Select(app => app.MapToFrontModel()).ToArrayAsync();

            var settings = dbContext.Settings
                .Include(x => x.Applications).AsQueryable();

            if (!string.IsNullOrEmpty(applicationName))
            {
                settings = settings.Where(x => x.Applications.Select(app => app.Name).Contains(applicationName));
            }

            var settingFrontModels = await settings.Select(x => x.MapToFrontModel()).ToArrayAsync();

            var model = new SettingFrontModel
            {
                AllApplications = apps,
                Settings = settingFrontModels
            };

            return OperationResult<SettingFrontModel>.Success(model);
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось получить список настроек";
            _logger.LogError(ex, errorMessage);
            return OperationResult<SettingFrontModel>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult<SettingItemFrontModel>> Add(SettingItemFrontModel setting, CancellationToken cancellationToken = default)
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var newApps = await dbContext.Applications.Where(x => setting.ApplicationNames.Contains(x.Name)).ToListAsync();

            var newSetting = new Setting
            {
                Name = setting.Name,
                Description = setting.Description,
                Value = setting.Value,
                ValueType = setting.ValueType,
                Applications = newApps,
                ExternalSourceKey = setting.ExternalSourceKey,
                ExternalSourcePath = setting.ExternalSourcePath,
                ExternalSourceType = setting.ExternalSourceType,
                IsFromExternalSource = setting.IsFromExternalSource,
            };

            await dbContext.Settings.AddAsync(newSetting);

            await dbContext.SaveChangesAsync();

            return OperationResult<SettingItemFrontModel>.Success(newSetting.MapToFrontModel());
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось добавить настройку";
            _logger.LogError(ex, errorMessage);
            return OperationResult<SettingItemFrontModel>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult<SettingItemFrontModel>> Edit(SettingItemFrontModel setting, CancellationToken cancellationToken = default)
    {
        if (setting.Key == null)
        {
            const string errorMessage = "Не указан ID редактируемой настройки";
            _logger.LogError(errorMessage);
            return OperationResult<SettingItemFrontModel>.Fail(string.Format(errorMessage));
        }

        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var currentSetting = await dbContext.Settings.Include(x => x.Applications).FirstOrDefaultAsync(x => x.Id == setting.Key);

            var newApps = await dbContext.Applications.Where(x => setting.ApplicationNames.Contains(x.Name)).ToArrayAsync();

            if (currentSetting == null)
            {
                const string errorMessageTemplate = "Настройка с ID = {0} не найдена в базе";
                _logger.LogError(errorMessageTemplate, setting.Key);
                return OperationResult<SettingItemFrontModel>.Fail(string.Format(errorMessageTemplate, setting.Key));
            }

            currentSetting.Name = setting.Name;
            currentSetting.Description = setting.Description;
            currentSetting.Applications.Clear();
            currentSetting.Applications.AddRange(newApps);
            currentSetting.IsFromExternalSource = setting.IsFromExternalSource;

            if (setting.IsFromExternalSource)
            {
                currentSetting.Value = null;
                currentSetting.ExternalSourcePath = setting.ExternalSourcePath;
                currentSetting.ExternalSourceKey = setting.ExternalSourceKey;
                currentSetting.ExternalSourceType = setting.ExternalSourceType;
            }
            else
            {
                currentSetting.Value = setting.Value;
                currentSetting.ExternalSourcePath = null;
                currentSetting.ExternalSourceKey = null;
                currentSetting.ExternalSourceType = null;
            }

            currentSetting.ValueType = setting.ValueType;

            await dbContext.SaveChangesAsync();

            return OperationResult<SettingItemFrontModel>.Success(currentSetting.MapToFrontModel());
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось отредактировать настройку";
            _logger.LogError(ex, errorMessage);
            return OperationResult<SettingItemFrontModel>.Fail(errorMessage);
        }
    }

    public async Task<OperationResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var currentSetting = await dbContext.Settings.FirstOrDefaultAsync(x => x.Id == id);

            if (currentSetting == null)
            {
                const string errorMessageTemplate = "Настройка с ID = {0} не найдена в базе";
                _logger.LogError(errorMessageTemplate, id);
                return OperationResult.Fail(string.Format(errorMessageTemplate, id));
            }

            dbContext.Settings.Remove(currentSetting);

            await dbContext.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось удалить настройку";
            _logger.LogError(ex, errorMessage);
            return OperationResult.Fail(errorMessage);
        }
    }
}
