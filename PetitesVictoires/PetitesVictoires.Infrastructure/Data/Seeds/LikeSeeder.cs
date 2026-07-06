using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.LikeAggregate;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public class LikeSeeder
{
    private static readonly LikeId LikeId1 = LikeId.From(1);
    private static readonly LikeId LikeId2 = LikeId.From(2);

    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext)
    {
        if (await dbContext.Likes.AnyAsync()) return; // DB has been seeded

        await PopulateDataAsync(dbContext);
    }

    private static async Task PopulateDataAsync(PetitesVictoiresDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Likes\" (\"Id\", \"UserId\", \"PostId\", \"CreatedAt\") VALUES ({LikeId1.Value}, {UserSeeder.UserId1.Value}, {PostSeeder.PostId1.Value}, {DateTime.UtcNow})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Likes\" (\"Id\", \"UserId\", \"PostId\", \"CreatedAt\") VALUES ({LikeId2.Value}, {UserSeeder.UserId2.Value}, {PostSeeder.PostId2.Value}, {DateTime.UtcNow})");
    }
}
