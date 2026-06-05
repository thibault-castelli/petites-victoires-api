using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Infrastructure.Data;

namespace PetitesVictoires.Api.Configurations;

public static class DatabaseConfiguration
{
    extension(WebApplication app)
    {
        public async Task MigrateDatabaseAsync()
        {
            var shouldMigrate = app.Environment.IsDevelopment();
            if (!shouldMigrate) return;

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Migrating database...");
                var context = services.GetRequiredService<PetitesVictoiresDbContext>();
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database. {ExMessage}", ex.Message);
                throw; // Re-throw to make startup fail if migrations fail
            }
        }
    }
}
