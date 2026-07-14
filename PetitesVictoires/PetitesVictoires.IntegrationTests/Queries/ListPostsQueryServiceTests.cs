using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Infrastructure.Queries;
using PetitesVictoires.UseCases.Common;
using PetitesVictoires.UseCases.Posts.List;
using Shouldly;

namespace PetitesVictoires.IntegrationTests.Queries;

[TestFixture]
public class ListPostsQueryServiceTests : IntegrationTestBase
{
    private ListPostsQueryService _sut = null!;

    [SetUp]
    public void CreateSut() => _sut = new ListPostsQueryService(DbContext);

    private static ListQueryParams Paging(int page = 1, int countPerPage = 10) => new(page, countPerPage);

    [Test]
    public async Task ListAsync_TotalEntityCount_EqualsNumberOfPosts()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(1, "one", 1);
        await SeedPostAsync(2, "two", 1);
        await SeedPostAsync(3, "three", 1);

        var result = await _sut.ListAsync(Paging(), new ListPostsCriteria(PostSortBy.CreatedAt), CancellationToken.None);

        result.TotalEntityCount.ShouldBe(3);
    }

    [Test]
    public async Task ListAsync_SortByCreatedAt_OrdersNewestFirst()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(1, "oldest", 1, DateTime.UtcNow.AddDays(-3));
        await SeedPostAsync(2, "middle", 1, DateTime.UtcNow.AddDays(-2));
        await SeedPostAsync(3, "newest", 1, DateTime.UtcNow.AddDays(-1));

        var result = await _sut.ListAsync(Paging(), new ListPostsCriteria(PostSortBy.CreatedAt), CancellationToken.None);

        result.Items.Select(p => p.Content.Value).ShouldBe(["newest", "middle", "oldest"]);
    }

    [Test]
    public async Task ListAsync_SortByLikesCount_OrdersMostLikedFirst()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedPostAsync(1, "few likes", 1);
        await SeedPostAsync(2, "many likes", 1);
        await SeedLikeAsync(1, 1, 2);
        await SeedLikeAsync(2, 2, 2);
        await SeedLikeAsync(3, 2, 1);

        var result = await _sut.ListAsync(Paging(), new ListPostsCriteria(PostSortBy.LikesCount), CancellationToken.None);

        result.Items.Select(p => p.Content.Value).ShouldBe(["many likes", "few likes"]);
    }

    [Test]
    public async Task ListAsync_WhenCreatedByProvided_ReturnsOnlyThatUsersPosts()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedPostAsync(1, "by alice", 1);
        await SeedPostAsync(2, "by bob", 2);

        var result = await _sut.ListAsync(Paging(),
            new ListPostsCriteria(PostSortBy.CreatedAt, CreatedBy: UserId.From(1)), CancellationToken.None);

        result.Items.Select(p => p.Content.Value).ShouldBe(["by alice"]);
    }

    [Test]
    public async Task ListAsync_WhenLikedByProvided_ReturnsOnlyPostsThatUserLiked()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedPostAsync(1, "liked by bob", 1);
        await SeedPostAsync(2, "not liked by bob", 1);
        await SeedLikeAsync(1, 2, 1);

        var result = await _sut.ListAsync(Paging(),
            new ListPostsCriteria(PostSortBy.CreatedAt, LikedBy: UserId.From(2)), CancellationToken.None);

        result.Items.Select(p => p.Content.Value).ShouldBe(["liked by bob"]);
    }

    [Test]
    public async Task ListAsync_MapsLikesCountPerPost()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedPostAsync(1, "content", 1);
        await SeedLikeAsync(1, 1, 1);
        await SeedLikeAsync(2, 2, 1);

        var result = await _sut.ListAsync(Paging(), new ListPostsCriteria(PostSortBy.CreatedAt), CancellationToken.None);

        result.Items.Single().LikesCount.ShouldBe(2);
    }

    [Test]
    public async Task ListAsync_SecondPage_SkipsFirstPageItems()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(1, "oldest", 1, DateTime.UtcNow.AddDays(-3));
        await SeedPostAsync(2, "middle", 1, DateTime.UtcNow.AddDays(-2));
        await SeedPostAsync(3, "newest", 1, DateTime.UtcNow.AddDays(-1));

        var result = await _sut.ListAsync(Paging(page: 2, countPerPage: 2),
            new ListPostsCriteria(PostSortBy.CreatedAt), CancellationToken.None);

        result.Items.Single().Content.Value.ShouldBe("oldest");
    }

    [Test]
    public async Task ListAsync_ComputesTotalPagesFromCountPerPage()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(1, "one", 1);
        await SeedPostAsync(2, "two", 1);
        await SeedPostAsync(3, "three", 1);

        var result = await _sut.ListAsync(Paging(page: 1, countPerPage: 2),
            new ListPostsCriteria(PostSortBy.CreatedAt), CancellationToken.None);

        result.TotalPages.ShouldBe(2);
    }

    [Test]
    public async Task ListAsync_ExcludesSoftDeletedPosts()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(1, "visible", 1);
        await SeedPostAsync(2, "deleted", 1);
        await SoftDeletePostAsync(2);

        var result = await _sut.ListAsync(Paging(), new ListPostsCriteria(PostSortBy.CreatedAt), CancellationToken.None);

        result.Items.Select(p => p.Content.Value).ShouldBe(["visible"]);
    }

    [Test]
    public async Task ListAsync_MapsPostAndAuthorFields()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedPostAsync(10, "my victory", 1);

        var result = await _sut.ListAsync(Paging(), new ListPostsCriteria(PostSortBy.CreatedAt), CancellationToken.None);

        var dto = result.Items.Single();
        dto.Id.Value.ShouldBe(10);
        dto.Content.Value.ShouldBe("my victory");
        dto.UserId.Value.ShouldBe(1);
        dto.UserEmailAddress.Value.ShouldBe("alice@mail.com");
        dto.UserName.Value.ShouldBe("alice");
    }
}
