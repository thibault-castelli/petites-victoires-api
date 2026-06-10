using Ardalis.ListStartupServices;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.Infrastructure.Data.Seeds;
using Scalar.AspNetCore;

namespace PetitesVictoires.Api.Configurations;

public static class MiddlewareConfigurations
{
    extension(WebApplication app)
    {
        public async Task<IApplicationBuilder> AddMidlewareConfigurations()
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
            }
            else
            {
                app.UseDefaultExceptionHandler(); // from FastEndpoints
                app.UseHsts();
            }

            app.UseFastEndpoints();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerGen(options => { options.Path = "/openapi/{documentName}.json"; },
                    settings =>
                    {
                        settings.Path = "/swagger";
                        settings.DocumentPath = "/openapi/{documentName}.json";
                    });

                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Petites Victoires API");
                    options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
                });
            }

            app.UseHttpsRedirection(); // Note this will drop Authorization headers

            var shouldMigrate = app.Environment.IsDevelopment();
            if (shouldMigrate)
            {
                await app.MigrateDatabaseAsync();
                await app.SeedAsync();
            }

            return app;
        }

        public async Task MigrateDatabaseAsync()
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Migrating database...");
                var context = services.GetRequiredService<PetitesVictoiresDbContext>();
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrated database successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database. {Message}", ex.Message);
                throw; // Re-throw to make startup fail if migrations fail
            }
        }

        public async Task SeedAsync()
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Seeding database...");
                var context = services.GetRequiredService<PetitesVictoiresDbContext>();
                await DatabaseSeeder.SeedAsync(context);
                logger.LogInformation("Seeding database completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database. {Message}", ex.Message);
            }
        }
    }
}
