namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext)
    {
        await UserSeeder.SeedAsync(dbContext);
        await PostSeeder.SeedAsync(dbContext);
        await LikeSeeder.SeedAsync(dbContext);
    }
}
