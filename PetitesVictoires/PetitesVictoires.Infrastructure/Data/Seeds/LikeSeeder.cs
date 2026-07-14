using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public class LikeSeeder
{
    private static readonly LikeId LikeId1 = LikeId.From(1);
    private static readonly LikeId LikeId2 = LikeId.From(2);

    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext, List<UserId> userIds)
    {
        if (await dbContext.Likes.AnyAsync()) return; // DB has been seeded

        await PopulateDataAsync(dbContext, userIds);
        await FastForwardSequenceId(dbContext);
    }

    private static async Task PopulateDataAsync(PetitesVictoiresDbContext dbContext, List<UserId> userIds)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Likes\" (\"Id\", \"UserId\", \"PostId\", \"CreatedAt\") VALUES ({LikeId1.Value}, {userIds[0].Value}, {PostSeeder.PostId1.Value}, {DateTime.UtcNow})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Likes\" (\"Id\", \"UserId\", \"PostId\", \"CreatedAt\") VALUES ({LikeId2.Value}, {userIds[1].Value}, {PostSeeder.PostId2.Value}, {DateTime.UtcNow})");
    }

    private static async Task FastForwardSequenceId(PetitesVictoiresDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT setval(pg_get_serial_sequence('\"Likes\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Likes\"), true);"
        );
    }
}
