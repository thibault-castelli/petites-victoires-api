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

// Top-level statements compile to an internal Program; this exposes it so the functional test
// project can boot the real app through WebApplicationFactory<Program>.
public partial class Program;
