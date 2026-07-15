using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.TestSupport;

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

    protected Task SeedUserAsync(int id, string email, string name, DateTime? createdAt = null) =>
        DbContext.SeedUserAsync(id, email, name, createdAt);

    protected Task SeedPostAsync(int id, string content, int userId, DateTime? createdAt = null) =>
        DbContext.SeedPostAsync(id, content, userId, createdAt);

    protected Task SeedLikeAsync(int id, int userId, int postId, DateTime? createdAt = null) =>
        DbContext.SeedLikeAsync(id, userId, postId, createdAt);

    protected Task SoftDeletePostAsync(int id) => DbContext.SoftDeletePostAsync(id);
}
