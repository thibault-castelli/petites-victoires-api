using FastEndpoints;
using FastEndpoints.Swagger;
using PetitesVictoires.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults() // OpenTelemetry logging
    .AddLoggerConfigurations(); // This adds Serilog for console formatting

builder.AddRedisDistributedCache("cache");

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

builder.Services.AddServiceConfigurations(startupLogger, builder);

builder.Services.AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "Petites Victoires API";
            s.Version = "v1";
            s.Description = "Http endpoints for the Petites Victoires API";
        };
        o.ShortSchemaNames = true;
    });

var app = builder.Build();

await app.AddMidlewareConfigurations();
app.MapDefaultEndpoints(); // Aspire health checks and metrics

app.Run();
