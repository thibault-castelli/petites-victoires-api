namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext)
    {
        await PostSeeder.SeedAsync(dbContext);
    }
}
