using PetitesVictoires.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders(); // Drop default Console/Debug
builder.AddServiceDefaults() // OpenTelemetry logging
    .AddLoggerConfigurations(); // This adds Serilog for console formatting

builder.AddRedisDistributedCache("cache");

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

builder.Services.AddOptionConfigurations(startupLogger, builder);
builder.Services.AddServiceConfigurations(startupLogger, builder);

var app = builder.Build();

await app.AddMidlewareConfigurations();
app.MapDefaultEndpoints(); // Aspire health checks and metrics

app.Run();
