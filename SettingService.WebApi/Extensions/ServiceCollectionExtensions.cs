using Serilog.Events;
using Serilog;

namespace SettingService.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSerilogLogger(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();

            var logger = new LoggerConfiguration()
               .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
               .WriteTo.Console()
               .CreateLogger();

            builder.AddSerilog(logger);
        });

        return services;
    }
}
