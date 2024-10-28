using Microsoft.EntityFrameworkCore;
using SettingService.DataLayer;
using SettingService.Services.Interfaces;

namespace SettingService.Services.Implementation;

internal class SettingsService : ISettingsService
{
    private readonly IDbContextFactory<SettingsContext> _dbContextFactory;

    public SettingsService(IDbContextFactory<SettingsContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
}
