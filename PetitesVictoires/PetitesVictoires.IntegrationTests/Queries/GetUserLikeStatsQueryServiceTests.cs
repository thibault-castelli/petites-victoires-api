using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Infrastructure.Queries;
using Shouldly;

namespace PetitesVictoires.IntegrationTests.Queries;

[TestFixture]
public class GetUserLikeStatsQueryServiceTests : IntegrationTestBase
{
    private GetUserLikeStatsQueryService _sut = null!;

    [SetUp]
    public void CreateSut() => _sut = new GetUserLikeStatsQueryService(DbContext);

    [Test]
    public async Task GetUserLikeStatsAsync_CountsLikesGivenByUser()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedPostAsync(1, "bob's post", 2);
        await SeedPostAsync(2, "bob's other post", 2);
        await SeedLikeAsync(1, 1, 1);
        await SeedLikeAsync(2, 1, 2);

        var result = await _sut.GetUserLikeStatsAsync(UserId.From(1), CancellationToken.None);

        result.LikesGiven.ShouldBe(2);
    }

    [Test]
    public async Task GetUserLikeStatsAsync_LikesGiven_OnlyCountsThisUsersLikes()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedPostAsync(1, "post", 1);
        await SeedLikeAsync(1, 1, 1);
        await SeedLikeAsync(2, 2, 1);

        var result = await _sut.GetUserLikeStatsAsync(UserId.From(1), CancellationToken.None);

        result.LikesGiven.ShouldBe(1);
    }

    [Test]
    public async Task GetUserLikeStatsAsync_CountsLikesReceivedAcrossUsersPosts()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedUserAsync(3, "c@mail.com", "carol");
        await SeedPostAsync(1, "alice post 1", 1);
        await SeedPostAsync(2, "alice post 2", 1);
        await SeedLikeAsync(1, 2, 1);
        await SeedLikeAsync(2, 3, 1);
        await SeedLikeAsync(3, 2, 2);

        var result = await _sut.GetUserLikeStatsAsync(UserId.From(1), CancellationToken.None);

        result.LikesReceived.ShouldBe(3);
    }

    [Test]
    public async Task GetUserLikeStatsAsync_LikesReceived_OnlyCountsThisUsersPosts()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedPostAsync(1, "alice post", 1);
        await SeedPostAsync(2, "bob post", 2);
        await SeedLikeAsync(1, 2, 1);
        await SeedLikeAsync(2, 1, 2);

        var result = await _sut.GetUserLikeStatsAsync(UserId.From(1), CancellationToken.None);

        result.LikesReceived.ShouldBe(1);
    }

    [Test]
    public async Task GetUserLikeStatsAsync_WhenUserHasNoActivity_ReturnsZeros()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");

        var result = await _sut.GetUserLikeStatsAsync(UserId.From(1), CancellationToken.None);

        result.LikesGiven.ShouldBe(0);
        result.LikesReceived.ShouldBe(0);
    }
}
