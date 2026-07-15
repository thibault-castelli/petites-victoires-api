using System.Net;
using System.Net.Http.Json;
using PetitesVictoires.Api.Users;
using PetitesVictoires.Api.Users.GetMe;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Users;

[TestFixture]
public class GetMeEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task GetCurrentUser_WhenAuthenticated_Returns200()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");

        var response = await CreateClientAs(1).GetAsync(GetMeEndpoint.Route);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetCurrentUser_WhenAuthenticated_ReturnsTheAuthenticatedUser()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");

        var user = await CreateClientAs(2).GetFromJsonAsync<UserRecord>(GetMeEndpoint.Route);

        user!.Id.ShouldBe(2);
        user.EmailAddress.ShouldBe("bob@mail.com");
        user.Name.ShouldBe("bob");
    }

    [Test]
    public async Task GetCurrentUser_WhenNotAuthenticated_Returns401()
    {
        var response = await CreateClient().GetAsync(GetMeEndpoint.Route);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GetCurrentUser_WhenAuthenticatedUserNoLongerExists_Returns404()
    {
        var response = await CreateClientAs(999).GetAsync(GetMeEndpoint.Route);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
