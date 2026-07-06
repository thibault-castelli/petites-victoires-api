using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class PostSeeder
{
    private static readonly PostId PostId1 = PostId.From(1);
    private static readonly PostId PostId2 = PostId.From(2);

    private static readonly PostContent PostContent1 = PostContent.From("Hello world!");
    private static readonly PostContent PostContent2 = PostContent.From("Hi there!");

    private static readonly UserId UserId1 = UserId.From(1);
    private static readonly UserId UserId2 = UserId.From(2);

    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext)
    {
        if (await dbContext.Posts.AnyAsync()) return; // DB has been seeded

        await PopulateDataAsync(dbContext);
    }

    private static async Task PopulateDataAsync(PetitesVictoiresDbContext dbContext)
    {
        // Use SQL inserts to avoid key generation/conversion issues with value object IDs.
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Posts\" (\"Id\", \"Content\", \"UserId\", \"CreatedAt\") VALUES ({PostId1.Value}, {PostContent1.Value}, {UserId1.Value}, {DateTime.UtcNow})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Posts\" (\"Id\", \"Content\", \"UserId\", \"CreatedAt\") VALUES ({PostId2.Value}, {PostContent2.Value}, {UserId2.Value}, {DateTime.UtcNow})");
    }
}
