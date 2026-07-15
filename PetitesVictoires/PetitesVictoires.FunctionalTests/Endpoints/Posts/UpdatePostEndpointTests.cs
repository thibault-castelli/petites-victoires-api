using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Api.Posts;
using PetitesVictoires.Api.Posts.Update;
using PetitesVictoires.Core.PostAggregate;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Posts;

[TestFixture]
public class UpdatePostEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task UpdatePost_WhenAnonymous_Returns401()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "original", 1);

        var response = await CreateClient()
            .PutAsJsonAsync(UpdatePostRequest.BuildRoute(10), new { postContent = "edited" });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task UpdatePost_WhenOwner_Returns200()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "original", 1);

        var response = await CreateClientAs(1)
            .PutAsJsonAsync(UpdatePostRequest.BuildRoute(10), new { postContent = "edited" });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task UpdatePost_WhenOwner_ReturnsUpdatedContent()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "original", 1);

        var response = await CreateClientAs(1)
            .PutAsJsonAsync(UpdatePostRequest.BuildRoute(10), new { postContent = "edited" });

        var updated = await response.Content.ReadFromJsonAsync<PostRecord>();
        updated!.Content.ShouldBe("edited");
    }

    [Test]
    public async Task UpdatePost_WhenOwner_PersistsNewContent()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "original", 1);

        await CreateClientAs(1).PutAsJsonAsync(UpdatePostRequest.BuildRoute(10), new { postContent = "edited" });

        await using var verify = DatabaseFixture.CreateContext();
        var post = await verify.Posts.FirstAsync(p => p.Id == PostId.From(10));
        post.Content.Value.ShouldBe("edited");
    }

    [Test]
    public async Task UpdatePost_WhenNotOwner_Returns403()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "alice's post", 1);

        var response = await CreateClientAs(2)
            .PutAsJsonAsync(UpdatePostRequest.BuildRoute(10), new { postContent = "hijacked" });

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task UpdatePost_WhenPostDoesNotExist_Returns404()
    {
        await SeedUserAsync(1);

        var response = await CreateClientAs(1)
            .PutAsJsonAsync(UpdatePostRequest.BuildRoute(999), new { postContent = "edited" });

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdatePost_WhenContentEmpty_Returns400()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "original", 1);

        var response = await CreateClientAs(1)
            .PutAsJsonAsync(UpdatePostRequest.BuildRoute(10), new { postContent = "" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
