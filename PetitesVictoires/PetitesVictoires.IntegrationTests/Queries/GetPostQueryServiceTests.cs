using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Infrastructure.Queries;
using Shouldly;

namespace PetitesVictoires.IntegrationTests.Queries;

[TestFixture]
public class GetPostQueryServiceTests : IntegrationTestBase
{
    private GetPostQueryService _sut = null!;

    [SetUp]
    public void CreateSut() => _sut = new GetPostQueryService(DbContext);

    [Test]
    public async Task GetPostAsync_WhenPostExists_MapsPostAndAuthorFields()
    {
        var createdAt = DateTime.UtcNow.AddDays(-1);
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedPostAsync(10, "my victory", 1, createdAt);

        var result = await _sut.GetPostAsync(PostId.From(10), CancellationToken.None);

        result.ShouldNotBeNull();
        result.Id.Value.ShouldBe(10);
        result.Content.Value.ShouldBe("my victory");
        result.UserId.Value.ShouldBe(1);
        result.UserEmailAddress.Value.ShouldBe("alice@mail.com");
        result.UserName.Value.ShouldBe("alice");
        result.CreatedAt.ShouldBe(createdAt, TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task GetPostAsync_WhenPostHasLikes_CountsThem()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);
        await SeedLikeAsync(1, 1, 10);
        await SeedLikeAsync(2, 2, 10);

        var result = await _sut.GetPostAsync(PostId.From(10), CancellationToken.None);

        result!.LikesCount.ShouldBe(2);
    }

    [Test]
    public async Task GetPostAsync_WhenPostHasNoLikes_ReturnsZeroLikesCount()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);

        var result = await _sut.GetPostAsync(PostId.From(10), CancellationToken.None);

        result!.LikesCount.ShouldBe(0);
    }

    [Test]
    public async Task GetPostAsync_WhenPostDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetPostAsync(PostId.From(999), CancellationToken.None);

        result.ShouldBeNull();
    }

    [Test]
    public async Task GetPostAsync_WhenPostSoftDeleted_ReturnsNull()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);
        await SoftDeletePostAsync(10);

        var result = await _sut.GetPostAsync(PostId.From(10), CancellationToken.None);

        result.ShouldBeNull();
    }
}
