using System.Net;
using System.Net.Http.Json;
using PetitesVictoires.Api.Posts;
using PetitesVictoires.Api.Posts.Get;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Posts;

[TestFixture]
public class GetPostByIdEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task GetPostById_WhenPostExists_Returns200()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "content", 1);

        var response = await CreateClient().GetAsync(GetPostByIdRequest.BuildRoute(10));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetPostById_WhenPostExists_ReturnsPostWithAuthor()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);

        var post = await CreateClient().GetFromJsonAsync<PostRecord>(GetPostByIdRequest.BuildRoute(10));

        post!.Id.ShouldBe(10);
        post.Content.ShouldBe("content");
        post.UserId.ShouldBe(1);
        post.UserEmailAddress.ShouldBe("alice@mail.com");
        post.UserName.ShouldBe("alice");
    }

    [Test]
    public async Task GetPostById_WhenPostHasLikes_ReturnsLikesCount()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);
        await SeedLikeAsync(1, 1, 10);
        await SeedLikeAsync(2, 2, 10);

        var post = await CreateClient().GetFromJsonAsync<PostRecord>(GetPostByIdRequest.BuildRoute(10));

        post!.LikesCount.ShouldBe(2);
    }

    [Test]
    public async Task GetPostById_WhenPostDoesNotExist_Returns404()
    {
        var response = await CreateClient().GetAsync(GetPostByIdRequest.BuildRoute(999));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetPostById_WhenPostSoftDeleted_Returns404()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "content", 1);
        await SoftDeletePostAsync(10);

        var response = await CreateClient().GetAsync(GetPostByIdRequest.BuildRoute(10));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
