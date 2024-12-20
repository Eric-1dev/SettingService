﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SettingService.Cache;
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
    private readonly IServiceProvider _serviceProvider;
    private readonly ICacheService _cacheService;

    public SettingsService(IDbContextFactory<SettingsContext> dbContextFactory,
        ILogger<SettingsUIService> logger,
        IServiceProvider serviceProvider,
        ICacheService cacheService)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _cacheService = cacheService;
    }

    public async Task InitializeCache(CancellationToken cancellationToken)
    {
        using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var settings = await dbContext.Settings.ToArrayAsync(cancellationToken);

        var settingItems = new List<SettingItem>();

        foreach (var setting in settings)
        {
            var settingItem = await GetSettingItemFromDao(setting);

            settingItems.Add(settingItem);
        }

        _cacheService.Initialize(settingItems);
    }

    public async Task<OperationResult<IReadOnlyCollection<SettingItem>>> GetAll(string applicationName, CancellationToken cancellationToken)
    {
        try
        {
            applicationName = applicationName.ToLower();

            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var settingNames = await dbContext.Settings
                .Where(x => x.Applications.Select(app => app.Name.ToLower()).Contains(applicationName))
                .Select(x => x.Name)
                .ToArrayAsync(cancellationToken);

            var settingItems = _cacheService.Get(settingNames);

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
                .Where(x => x.Name == settingName && x.Applications.Select(app => app.Name.ToLower()).Contains(applicationName))
                .FirstOrDefaultAsync(cancellationToken);

            if (setting == null)
            {
                const string errorMessage = "Не найдена настройка {0} для приложения {1}";
                _logger.LogError(errorMessage, applicationName, settingName);
                return OperationResult<SettingItem>.Fail(string.Format(errorMessage, settingName, applicationName));
            }

            var settingItem = _cacheService.Get(setting.Name);

            if (settingItem == null)
                settingItem = await GetSettingItemFromDao(setting);

            return OperationResult<SettingItem>.Success(settingItem);
        }
        catch (Exception ex)
        {
            const string errorMessage = "Не удалось получить настройку";
            _logger.LogError(ex, errorMessage);
            return OperationResult<SettingItem>.Fail(ex.ToString());
        }
    }

    public async Task<SettingItem> GetSettingItemFromDao(Setting setting)
    {
        var settingItem = new SettingItem
        {
            Name = setting.Name,
            ValueType = setting.ValueType
        };

        if (setting.IsFromExternalSource)
        {
            settingItem.Value = await GetValueFromExternalSource(setting.ExternalSourceType, setting.ExternalSourcePath, setting.ExternalSourceKey);
        }
        else
        {
            settingItem.Value = setting.Value;
        }

        return settingItem;
    }

    private async Task<string?> GetValueFromExternalSource(ExternalSourceTypeEnum? externalSourceType, string? path, string? key)
    {
        var service = _serviceProvider.GetServices<IExternalSourceService>().First(x => x.ExternalSourceType == externalSourceType);

        var value = await service.GetSettingValueAsync(path!, key);

        return value;
    }
}
