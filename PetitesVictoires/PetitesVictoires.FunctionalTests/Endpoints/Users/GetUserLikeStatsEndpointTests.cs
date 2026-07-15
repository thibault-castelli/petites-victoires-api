using System.Net;
using System.Net.Http.Json;
using PetitesVictoires.Api.Users;
using PetitesVictoires.Api.Users.GetLikeStats;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Users;

[TestFixture]
public class GetUserLikeStatsEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task GetUserLikeStats_WhenAnonymous_Returns401()
    {
        await SeedUserAsync(1);

        var response = await CreateClient().GetAsync(GetUserLikeStatsRequest.BuildRoute(1));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GetUserLikeStats_CountsLikesGivenByUser()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "alice's post", 1);
        await SeedPostAsync(11, "bob's post", 2);
        await SeedLikeAsync(1, 1, 11);

        var stats = await CreateClientAs(1)
            .GetFromJsonAsync<UserLikeStatsRecord>(GetUserLikeStatsRequest.BuildRoute(1));

        stats!.LikesGiven.ShouldBe(1);
    }

    [Test]
    public async Task GetUserLikeStats_CountsLikesReceivedOnUsersPosts()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedPostAsync(10, "alice's post", 1);
        await SeedLikeAsync(1, 2, 10);

        var stats = await CreateClientAs(1)
            .GetFromJsonAsync<UserLikeStatsRecord>(GetUserLikeStatsRequest.BuildRoute(1));

        stats!.LikesReceived.ShouldBe(1);
    }

    [Test]
    public async Task GetUserLikeStats_WhenNoActivity_ReturnsZeros()
    {
        await SeedUserAsync(1);

        var stats = await CreateClientAs(1)
            .GetFromJsonAsync<UserLikeStatsRecord>(GetUserLikeStatsRequest.BuildRoute(1));

        stats!.LikesGiven.ShouldBe(0);
        stats.LikesReceived.ShouldBe(0);
    }
}
