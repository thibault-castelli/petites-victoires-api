using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Infrastructure.Data;

namespace PetitesVictoires.IntegrationTests;

public abstract class IntegrationTestBase
{
    protected PetitesVictoiresDbContext DbContext = null!;

    [SetUp]
    public async Task BaseSetUp()
    {
        await DatabaseFixture.ResetAsync();
        DbContext = DatabaseFixture.CreateContext();
    }

    [TearDown]
    public async Task BaseTearDown() => await DbContext.DisposeAsync();

    // Raw SQL inserts mirror the seeders: they set explicit ids into the Vogen-backed identity
    // columns and let us pin CreatedAt for deterministic ordering assertions.
    protected Task SeedUserAsync(int id, string email, string name, DateTime? createdAt = null) =>
        DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Users\" (\"Id\", \"EmailAddress\", \"Name\", \"CreatedAt\") VALUES ({id}, {email}, {name}, {createdAt ?? DateTime.UtcNow})");

    protected Task SeedPostAsync(int id, string content, int userId, DateTime? createdAt = null) =>
        DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Posts\" (\"Id\", \"Content\", \"UserId\", \"CreatedAt\") VALUES ({id}, {content}, {userId}, {createdAt ?? DateTime.UtcNow})");

    protected Task SeedLikeAsync(int id, int userId, int postId, DateTime? createdAt = null) =>
        DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"Likes\" (\"Id\", \"UserId\", \"PostId\", \"CreatedAt\") VALUES ({id}, {userId}, {postId}, {createdAt ?? DateTime.UtcNow})");

    protected Task SoftDeletePostAsync(int id) =>
        DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"Posts\" SET \"DeletedAt\" = {DateTime.UtcNow} WHERE \"Id\" = {id}");
}
