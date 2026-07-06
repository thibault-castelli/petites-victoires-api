using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public class LikeSeeder
{
    private static readonly LikeId LikeId1 = LikeId.From(1);
    private static readonly LikeId LikeId2 = LikeId.From(2);

    private static readonly UserId UserId1 = UserId.From(1);
    private static readonly UserId UserId2 = UserId.From(2);

    private static readonly PostId PostId1 = PostId.From(1);
    private static readonly PostId PostId2 = PostId.From(2);

    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext)
    {
        if (await dbContext.Likes.AnyAsync()) return; // DB has been seeded

        await PopulateDataAsync(dbContext);
    }

    private static async Task PopulateDataAsync(PetitesVictoiresDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Likes\" (\"Id\", \"UserId\", \"PostId\", \"CreatedAt\") VALUES ({LikeId1.Value}, {UserId1.Value}, {PostId1.Value}, {DateTime.UtcNow})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Likes\" (\"Id\", \"UserId\", \"PostId\", \"CreatedAt\") VALUES ({LikeId2.Value}, {UserId2.Value}, {PostId2.Value}, {DateTime.UtcNow})");
    }
}
