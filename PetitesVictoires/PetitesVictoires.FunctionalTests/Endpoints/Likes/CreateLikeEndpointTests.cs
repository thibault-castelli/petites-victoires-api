using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Api.Likes.Create;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Likes;

[TestFixture]
public class CreateLikeEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task CreateLike_WhenAnonymous_Returns401()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "content", 1);

        var response = await CreateClient().PostAsJsonAsync(CreateLikeRequest.Route, new { postId = 10 });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task CreateLike_WhenAuthenticated_Returns201()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);

        var response = await CreateClientAs(2).PostAsJsonAsync(CreateLikeRequest.Route, new { postId = 10 });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Test]
    public async Task CreateLike_WhenAuthenticated_PersistsLike()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);

        await CreateClientAs(2).PostAsJsonAsync(CreateLikeRequest.Route, new { postId = 10 });

        await using var verify = DatabaseFixture.CreateContext();
        var like = await verify.Likes.SingleAsync();
        like.PostId.Value.ShouldBe(10);
        like.UserId.Value.ShouldBe(2);
    }

    // Conflict is funnelled through ToCreatedResult's catch-all, which problems out as 400 (not 409).
    [Test]
    public async Task CreateLike_WhenAlreadyLiked_Returns400()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);
        await SeedLikeAsync(1, 2, 10);

        var response = await CreateClientAs(2).PostAsJsonAsync(CreateLikeRequest.Route, new { postId = 10 });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateLike_WhenPostDoesNotExist_Returns400()
    {
        await SeedUserAsync(1);

        var response = await CreateClientAs(1).PostAsJsonAsync(CreateLikeRequest.Route, new { postId = 999 });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
