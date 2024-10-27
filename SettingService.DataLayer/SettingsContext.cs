using Microsoft.EntityFrameworkCore;
using SettingService.Entities;

namespace SettingService.DataLayer;

public class SettingsContext : DbContext
{
    public DbSet<Application> Applications { get; set; }

    public DbSet<Setting> Settings { get; set; }

    public SettingsContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite("C:/Work/SettingService/DataBase/SettingService.db");
    }
}
