using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.Infrastructure.Data.Seeds;

namespace PetitesVictoires.Api.Configurations;

public static class SeedConfiguration
{
    extension(WebApplication app)
    {
        public async Task SeedAsync()
        {
            var shouldSeed = app.Environment.IsDevelopment();
            if (!shouldSeed) return;

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Seeding database...");
                var context = services.GetRequiredService<PetitesVictoiresDbContext>();
                await DatabaseSeeder.SeedAsync(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database. {Message}", ex.Message);
            }
        }
    }
}
