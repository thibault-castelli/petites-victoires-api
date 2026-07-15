using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Api.Posts;
using PetitesVictoires.Api.Posts.Create;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Posts;

[TestFixture]
public class CreatePostEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task CreatePost_WhenAnonymous_Returns401()
    {
        var response = await CreateClient().PostAsJsonAsync(CreatePostRequest.Route, new { content = "hello" });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task CreatePost_WhenAuthenticated_Returns201()
    {
        await SeedUserAsync(1);

        var response = await CreateClientAs(1).PostAsJsonAsync(CreatePostRequest.Route, new { content = "my victory" });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Test]
    public async Task CreatePost_WhenAuthenticated_ReturnsCreatedPostInBody()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");

        var response = await CreateClientAs(1).PostAsJsonAsync(CreatePostRequest.Route, new { content = "my victory" });

        var created = await response.Content.ReadFromJsonAsync<PostRecord>();
        created!.Content.ShouldBe("my victory");
        created.UserId.ShouldBe(1);
        created.UserEmailAddress.ShouldBe("alice@mail.com");
        created.UserName.ShouldBe("alice");
    }

    [Test]
    public async Task CreatePost_WhenAuthenticated_PersistsPost()
    {
        await SeedUserAsync(1);

        await CreateClientAs(1).PostAsJsonAsync(CreatePostRequest.Route, new { content = "persisted" });

        await using var verify = DatabaseFixture.CreateContext();
        var post = await verify.Posts.SingleAsync();
        post.Content.Value.ShouldBe("persisted");
    }

    [Test]
    public async Task CreatePost_WhenContentEmpty_Returns400()
    {
        await SeedUserAsync(1);

        var response = await CreateClientAs(1).PostAsJsonAsync(CreatePostRequest.Route, new { content = "" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
