using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Infrastructure.Data;

namespace PetitesVictoires.TestSupport;

/// <summary>
///     Raw SQL inserts mirror the production seeders: they set explicit ids into the Vogen-backed
///     identity columns and let tests pin CreatedAt for deterministic ordering assertions.
///     Note these create domain rows only — endpoints that go through IIdentityService need a real
///     Identity user, created via POST /Users.
/// </summary>
public static class TestDataSeeder
{
    public static Task SeedUserAsync(this PetitesVictoiresDbContext dbContext, int id, string email, string name,
        DateTime? createdAt = null) =>
        dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Users\" (\"Id\", \"EmailAddress\", \"Name\", \"CreatedAt\") VALUES ({id}, {email}, {name}, {createdAt ?? DateTime.UtcNow})");

    public static Task SeedPostAsync(this PetitesVictoiresDbContext dbContext, int id, string content, int userId,
        DateTime? createdAt = null) =>
        dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Posts\" (\"Id\", \"Content\", \"UserId\", \"CreatedAt\") VALUES ({id}, {content}, {userId}, {createdAt ?? DateTime.UtcNow})");

    public static Task SeedLikeAsync(this PetitesVictoiresDbContext dbContext, int id, int userId, int postId,
        DateTime? createdAt = null) =>
        dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Likes\" (\"Id\", \"UserId\", \"PostId\", \"CreatedAt\") VALUES ({id}, {userId}, {postId}, {createdAt ?? DateTime.UtcNow})");

    public static Task SoftDeletePostAsync(this PetitesVictoiresDbContext dbContext, int id) =>
        dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"Posts\" SET \"DeletedAt\" = {DateTime.UtcNow} WHERE \"Id\" = {id}");
}
