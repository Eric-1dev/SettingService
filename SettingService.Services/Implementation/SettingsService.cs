using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SettingService.Contracts;
using SettingService.DataLayer;
using SettingService.DataModel;
using SettingService.Entities;
using SettingService.Services.Implementation.UI;
using SettingService.Services.Interfaces;
using SettingService.Services.Interfaces.ExternalSource;

namespace SettingService.Services.Implementation;

internal class SettingsService : ISettingsService
{
    private readonly IDbContextFactory<SettingsContext> _dbContextFactory;
    private readonly ILogger _logger;
    private IServiceProvider _serviceProvider;

    public SettingsService(IDbContextFactory<SettingsContext> dbContextFactory, ILogger<SettingsUIService> logger, IServiceProvider serviceProvider)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<OperationResult<IReadOnlyCollection<SettingItem>>> GetAll(string applicationName, CancellationToken cancellationToken)
    {
        try
        {
            applicationName = applicationName.ToLower();

            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var settings = await dbContext.Settings
                .Where(x => x.Applications.Select(app => app.Name.ToLower()).Contains(applicationName))
                .ToArrayAsync(cancellationToken);

            var settingItems = new List<SettingItem>();

            foreach (var setting in settings)
            {

                var settingItem = GetSettingItemFromDao(setting);

                settingItems.Add(settingItem);
            }

            return OperationResult<IReadOnlyCollection<SettingItem>>.Success(settingItems);
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось получить список настроек";
            _logger.LogError(ex, errorMessage);
            return OperationResult<IReadOnlyCollection<SettingItem>>.Fail(ex.ToString());
        }
    }

    public async Task<OperationResult<SettingItem>> GetByName(string applicationName, string settingName, CancellationToken cancellationToken)
    {
        try
        {
            applicationName = applicationName.ToLower();

            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var setting = await dbContext.Settings
                .Where(x => x.Applications.Select(app => app.Name.ToLower()).Contains(applicationName))
                .FirstOrDefaultAsync(cancellationToken);

            if (setting == null)
            {
                const string errorMessage = "Не найдена настройка {0} для приложения {1}";
                _logger.LogError(errorMessage, applicationName, settingName);
                return OperationResult<SettingItem>.Fail(string.Format(errorMessage, settingName, applicationName));
            }

            var settingItem = GetSettingItemFromDao(setting);

            return OperationResult<SettingItem>.Success(settingItem);
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось получить настройку";
            _logger.LogError(ex, errorMessage);
            return OperationResult<SettingItem>.Fail(ex.ToString());
        }
    }

    private SettingItem GetSettingItemFromDao(Setting setting)
    {
        var settingItem = new SettingItem
        {
            Name = setting.Name,
            ValueType = setting.ValueType
        };

        if (!setting.IsFromExternalSource)
        {
            settingItem.Value = setting.Value;
        }
        else
        {
            settingItem.Value = GetValueFromExternalSource(setting.ExternalSourceType, setting.ExternalSourcePath, setting.ExternalSourceKey);
        }

        return settingItem;
    }

    private string GetValueFromExternalSource(ExternalSourceTypeEnum? externalSourceType, string? path, string? key)
    {
        var service = _serviceProvider.GetServices<IExternalSourceService>().First(x => x.ExternalSourceType == externalSourceType);

        var value = service.GetSettingValue(path, key);

        return value;
    }
}
