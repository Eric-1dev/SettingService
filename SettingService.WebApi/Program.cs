using SettingService.DataLayer;
using SettingService.Services;
using SettingService.Services.Interfaces;
using SettingService.Services.Models;
using SettingService.WebApi.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitConfig>(builder.Configuration.GetSection("RabbitConfig"));
builder.Services.Configure<CryptoSettings>(builder.Configuration.GetSection("CryptoSettings"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddDbContextFactory<SettingsContext>()
    .AddCommonServices()
    .AddRabbit()
    .AddLogger();

var app = builder.Build();

app.UseRabbit();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(config =>
{
    config.AllowAnyOrigin();
    config.AllowAnyMethod();
    config.AllowAnyHeader();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<ISettingsService>()
    .InitializeCache(CancellationToken.None);

app.Run();
