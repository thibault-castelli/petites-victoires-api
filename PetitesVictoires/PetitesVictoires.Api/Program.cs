using FastEndpoints;
using FastEndpoints.Swagger;
using PetitesVictoires.Api.Configurations;
using PetitesVictoires.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(); // OpenTelemetry logging

builder.AddRedisDistributedCache("cache");

builder.AddPetitesVictoiresDbContext();

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

app.MapDefaultEndpoints(); // Aspire health checks and metrics
app.UseFastEndpoints();

await app.MigrateDatabaseAsync();

app.Run();
