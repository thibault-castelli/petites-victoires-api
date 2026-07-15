using System.Net;
using System.Net.Http.Json;
using PetitesVictoires.Api.Posts;
using PetitesVictoires.Api.Posts.List;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Posts;

[TestFixture]
public class ListPostsEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task ListPosts_WhenAnonymous_Returns200()
    {
        var response = await CreateClient().GetAsync(ListPostsRequest.Route);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task ListPosts_ReturnsTotalEntityCount()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(1, "first", 1, DateTime.UtcNow.AddDays(-2));
        await SeedPostAsync(2, "second", 1, DateTime.UtcNow.AddDays(-1));

        var body = await CreateClient().GetFromJsonAsync<PagedBody<PostRecord>>(ListPostsRequest.Route);

        body!.TotalEntityCount.ShouldBe(2);
    }

    [Test]
    public async Task ListPosts_SortedByCreatedAt_ReturnsNewestFirst()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(1, "older", 1, DateTime.UtcNow.AddDays(-2));
        await SeedPostAsync(2, "newer", 1, DateTime.UtcNow.AddDays(-1));

        var body = await CreateClient()
            .GetFromJsonAsync<PagedBody<PostRecord>>($"{ListPostsRequest.Route}?sort_by=created_at");

        body!.Items.Select(p => p.Content).ShouldBe(["newer", "older"]);
    }

    [Test]
    public async Task ListPosts_SortedByLikesCount_ReturnsMostLikedFirst()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(1, "unpopular", 1);
        await SeedPostAsync(2, "popular", 1);
        await SeedLikeAsync(1, 1, 2);
        await SeedLikeAsync(2, 2, 2);

        var body = await CreateClient()
            .GetFromJsonAsync<PagedBody<PostRecord>>($"{ListPostsRequest.Route}?sort_by=likes_count");

        body!.Items.Select(p => p.Content).ShouldBe(["popular", "unpopular"]);
    }

    [Test]
    public async Task ListPosts_WhenSortByNotAllowed_Returns400()
    {
        var response = await CreateClient().GetAsync($"{ListPostsRequest.Route}?sort_by=nonsense");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ListPosts_WhenPageBelowOne_Returns400()
    {
        var response = await CreateClient().GetAsync($"{ListPostsRequest.Route}?page=0");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ListPosts_WhenFilteredByLikedBy_ReturnsOnlyPostsThatUserLiked()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(1, "liked by bob", 1);
        await SeedPostAsync(2, "liked by nobody", 1);
        await SeedLikeAsync(1, 2, 1);

        var body = await CreateClient()
            .GetFromJsonAsync<PagedBody<PostRecord>>($"{ListPostsRequest.Route}?liked_by=2");

        body!.Items.Single().Content.ShouldBe("liked by bob");
    }

    [Test]
    public async Task ListPosts_WhenFilteredByCreatedBy_ReturnsOnlyThatUsersPosts()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(1, "by alice", 1);
        await SeedPostAsync(2, "by bob", 2);

        var body = await CreateClient()
            .GetFromJsonAsync<PagedBody<PostRecord>>($"{ListPostsRequest.Route}?created_by=2");

        body!.Items.Single().Content.ShouldBe("by bob");
    }

    [Test]
    public async Task ListPosts_WhenSecondPageRequested_SkipsFirstPage()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(1, "oldest", 1, DateTime.UtcNow.AddDays(-3));
        await SeedPostAsync(2, "middle", 1, DateTime.UtcNow.AddDays(-2));
        await SeedPostAsync(3, "newest", 1, DateTime.UtcNow.AddDays(-1));

        var body = await CreateClient()
            .GetFromJsonAsync<PagedBody<PostRecord>>($"{ListPostsRequest.Route}?page=2&count_per_page=2");

        body!.Items.Single().Content.ShouldBe("oldest");
    }

    [Test]
    public async Task ListPosts_ExcludesSoftDeletedPosts()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(1, "visible", 1);
        await SeedPostAsync(2, "deleted", 1);
        await SoftDeletePostAsync(2);

        var body = await CreateClient().GetFromJsonAsync<PagedBody<PostRecord>>(ListPostsRequest.Route);

        body!.Items.Single().Content.ShouldBe("visible");
    }
}
