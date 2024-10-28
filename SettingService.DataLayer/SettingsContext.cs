using Microsoft.EntityFrameworkCore;
using SettingService.DataModel;

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

        optionsBuilder.UseSqlite("Data Source=C:/Work/SettingService/Database/SettingService.db");
    }
}
