using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class PostSeeder
{
    private static readonly PostContent PostContent1 = PostContent.From("Hello world!");
    private static readonly PostContent PostContent2 = PostContent.From("Hi there!");

    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext)
    {
        if (await dbContext.Posts.AnyAsync()) return; // DB has been seeded

        await PopulateDataAsync(dbContext);
    }

    private static async Task PopulateDataAsync(PetitesVictoiresDbContext dbContext)
    {
        // Use SQL inserts to avoid key generation/conversion issues with value object IDs.
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Posts\" (\"Content\", \"CreatedAt\") VALUES ({PostContent1.Value}, {DateTime.UtcNow})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Posts\" (\"Content\", \"CreatedAt\") VALUES ({PostContent2.Value}, {DateTime.UtcNow})");
    }
}
