using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Data.Seeds;

public class LikeSeeder
{
    // (UserIndex, PostIndex) pairs — who liked which post, both 0-based into the seeded lists.
    // Popular posts get several likes, some posts get none, and nobody likes their own post.
    private static readonly (int UserIndex, int PostIndex)[] SeedLikes =
    [
        // Lucas' 5 km run (post #2) — everyone cheers
        (0, 1), (2, 1), (3, 1), (4, 1), (5, 1),
        // Emma's scary job application (post #9)
        (0, 8), (1, 8), (3, 8), (4, 8), (5, 8),
        // Camille's tarte tatin (post #1)
        (1, 0), (2, 0), (4, 0),
        // Thomas speaking up in a meeting (post #4)
        (1, 3), (2, 3), (4, 3),
        // Emma's inbox zero (post #14)
        (1, 13), (3, 13),
        // Léa's fully-checked to-do list (post #16)
        (0, 15), (2, 15),
        // Hugo's bike repair (post #6)
        (0, 5), (4, 5),
        // Camille saying no to a useless meeting (post #13)
        (2, 12), (4, 12),
        // Léa's first meditation (post #11)
        (2, 10),
        // Lucas cooking all week (post #8)
        (0, 7)
    ];

    public static async Task SeedAsync(PetitesVictoiresDbContext dbContext, List<UserId> userIds, List<PostId> postIds)
    {
        if (await dbContext.Likes.AnyAsync()) return; // DB has been seeded

        await PopulateDataAsync(dbContext, userIds, postIds);
        await FastForwardSequenceId(dbContext);
    }

    private static async Task PopulateDataAsync(PetitesVictoiresDbContext dbContext, List<UserId> userIds,
        List<PostId> postIds)
    {
        var now = DateTime.UtcNow;

        for (var i = 0; i < SeedLikes.Length; i++)
        {
            var (userIndex, postIndex) = SeedLikes[i];
            var id = i + 1;

            // Keep every like within the last few hours so it always lands after its post was created.
            var createdAt = now.AddMinutes(-10 * (SeedLikes.Length - i));

            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO \"Likes\" (\"Id\", \"UserId\", \"PostId\", \"CreatedAt\") VALUES ({id}, {userIds[userIndex].Value}, {postIds[postIndex].Value}, {createdAt})");
        }
    }

    private static async Task FastForwardSequenceId(PetitesVictoiresDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT setval(pg_get_serial_sequence('\"Likes\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Likes\"), true);"
        );
    }
}
