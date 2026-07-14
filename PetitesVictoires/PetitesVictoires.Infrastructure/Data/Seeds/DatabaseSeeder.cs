using Microsoft.AspNetCore.Identity;
using PetitesVictoires.Infrastructure.Identity;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        var userIds = await UserSeeder.SeedAsync(dbContext, userManager);
        await PostSeeder.SeedAsync(dbContext, userIds);
        await LikeSeeder.SeedAsync(dbContext, userIds);
    }
}
