using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Api.Likes.Delete;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Likes;

[TestFixture]
public class DeleteLikeEndpointTests : FunctionalTestBase
{
    // DELETE carries the post id in the body, so it needs an explicit request message.
    private static Task<HttpResponseMessage> DeleteLikeAsync(HttpClient client, int postId) =>
        client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, DeleteLikeRequest.Route)
        {
            Content = JsonContent.Create(new { postId })
        });

    [Test]
    public async Task DeleteLike_WhenAnonymous_Returns401()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "content", 1);

        var response = await DeleteLikeAsync(CreateClient(), 10);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task DeleteLike_WhenLikeExists_Returns204()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);
        await SeedLikeAsync(1, 2, 10);

        var response = await DeleteLikeAsync(CreateClientAs(2), 10);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task DeleteLike_WhenLikeExists_RemovesLike()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);
        await SeedLikeAsync(1, 2, 10);

        await DeleteLikeAsync(CreateClientAs(2), 10);

        await using var verify = DatabaseFixture.CreateContext();
        (await verify.Likes.AnyAsync()).ShouldBeFalse();
    }

    [Test]
    public async Task DeleteLike_WhenUserHasNotLikedPost_Returns404()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);

        var response = await DeleteLikeAsync(CreateClientAs(2), 10);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
