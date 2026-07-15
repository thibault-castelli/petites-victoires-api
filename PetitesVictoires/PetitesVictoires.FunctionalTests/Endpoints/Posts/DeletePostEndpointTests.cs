using System.Net;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Api.Posts.Delete;
using PetitesVictoires.Core.PostAggregate;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Posts;

[TestFixture]
public class DeletePostEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task DeletePost_WhenAnonymous_Returns401()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "content", 1);

        var response = await CreateClient().DeleteAsync(DeletePostRequest.BuildRoute(10));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task DeletePost_WhenOwner_Returns204()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "content", 1);

        var response = await CreateClientAs(1).DeleteAsync(DeletePostRequest.BuildRoute(10));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task DeletePost_WhenOwner_SoftDeletesRatherThanRemoves()
    {
        await SeedUserAsync(1);
        await SeedPostAsync(10, "content", 1);

        await CreateClientAs(1).DeleteAsync(DeletePostRequest.BuildRoute(10));

        await using var verify = DatabaseFixture.CreateContext();
        var post = await verify.Posts.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == PostId.From(10));
        post.ShouldNotBeNull();
        post.DeletedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task DeletePost_WhenNotOwner_Returns403()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "alice's post", 1);

        var response = await CreateClientAs(2).DeleteAsync(DeletePostRequest.BuildRoute(10));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task DeletePost_WhenPostDoesNotExist_Returns404()
    {
        await SeedUserAsync(1);

        var response = await CreateClientAs(1).DeleteAsync(DeletePostRequest.BuildRoute(999));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
