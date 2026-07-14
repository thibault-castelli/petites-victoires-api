using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public static class PostSeeder
{
    public static readonly PostId PostId1 = PostId.From(1);
    public static readonly PostId PostId2 = PostId.From(2);

    private static readonly PostContent PostContent1 = PostContent.From("Hello world!");
    private static readonly PostContent PostContent2 = PostContent.From("Hi there!");

    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext, List<UserId> userIds)
    {
        if (await dbContext.Posts.AnyAsync()) return; // DB has been seeded

        await PopulateDataAsync(dbContext, userIds);
        await FastForwardSequenceId(dbContext);
    }

    private static async Task PopulateDataAsync(PetitesVictoiresDbContext dbContext, List<UserId> userIds)
    {
        // Use SQL inserts to avoid key generation/conversion issues with value object IDs.
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Posts\" (\"Id\", \"Content\", \"UserId\", \"CreatedAt\") VALUES ({PostId1.Value}, {PostContent1.Value}, {userIds[0].Value}, {DateTime.UtcNow})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Posts\" (\"Id\", \"Content\", \"UserId\", \"CreatedAt\") VALUES ({PostId2.Value}, {PostContent2.Value}, {userIds[1].Value}, {DateTime.UtcNow})");
    }

    private static async Task FastForwardSequenceId(PetitesVictoiresDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT setval(pg_get_serial_sequence('\"Posts\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Posts\"), true);"
        );
    }
}
