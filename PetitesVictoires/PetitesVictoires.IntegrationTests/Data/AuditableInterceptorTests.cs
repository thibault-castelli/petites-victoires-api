using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Infrastructure.Data;
using Shouldly;

namespace PetitesVictoires.IntegrationTests.Data;

[TestFixture]
public class AuditableInterceptorTests : IntegrationTestBase
{
    private PetitesVictoiresDbContext _appContext = null!;

    [SetUp]
    public void CreateAppContext() => _appContext = DatabaseFixture.CreateContext(withInterceptors: true);

    [TearDown]
    public async Task DisposeAppContext() => await _appContext.DisposeAsync();

    [Test]
    public async Task SavingChanges_WhenAuditableModified_StampsUpdatedAt()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(10, "original", 1);

        var post = await _appContext.Posts.FirstAsync(p => p.Id == PostId.From(10));
        post.UpdateContent(PostContent.From("edited"));
        await _appContext.SaveChangesAsync();

        await using var verify = DatabaseFixture.CreateContext();
        var reloaded = await verify.Posts.FirstAsync(p => p.Id == PostId.From(10));
        reloaded.UpdatedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task SavingChanges_WhenInserted_LeavesUpdatedAtNull()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");

        _appContext.Posts.Add(new Post(PostContent.From("fresh"), UserId.From(1)));
        await _appContext.SaveChangesAsync();

        await using var verify = DatabaseFixture.CreateContext();
        var inserted = await verify.Posts.SingleAsync();
        inserted.UpdatedAt.ShouldBeNull();
    }

    [Test]
    public async Task DeleteAsync_OnSoftDeletable_KeepsRowAndStampsDeletedAt()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);

        var post = await _appContext.Posts.FirstAsync(p => p.Id == PostId.From(10));
        await new EfRepository<Post>(_appContext).DeleteAsync(post, CancellationToken.None);

        await using var verify = DatabaseFixture.CreateContext();
        var row = await verify.Posts.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == PostId.From(10));
        row.ShouldNotBeNull();
        row.DeletedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task DeleteAsync_OnSoftDeletable_HidesRowFromDefaultQuery()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);

        var post = await _appContext.Posts.FirstAsync(p => p.Id == PostId.From(10));
        await new EfRepository<Post>(_appContext).DeleteAsync(post, CancellationToken.None);

        await using var verify = DatabaseFixture.CreateContext();
        var visible = await verify.Posts.FirstOrDefaultAsync(p => p.Id == PostId.From(10));
        visible.ShouldBeNull();
    }
}
