using System.Net;
using System.Net.Http.Json;
using PetitesVictoires.Api.Users;
using PetitesVictoires.Api.Users.Get;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Users;

[TestFixture]
public class GetUserByIdEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task GetUserById_WhenUserExists_Returns200()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");

        var response = await CreateClient().GetAsync(GetUserByIdRequest.BuildRoute(1));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task GetUserById_WhenUserExists_ReturnsUser()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");

        var user = await CreateClient().GetFromJsonAsync<UserRecord>(GetUserByIdRequest.BuildRoute(1));

        user!.Id.ShouldBe(1);
        user.EmailAddress.ShouldBe("alice@mail.com");
        user.Name.ShouldBe("alice");
    }

    [Test]
    public async Task GetUserById_WhenUserDoesNotExist_Returns404()
    {
        var response = await CreateClient().GetAsync(GetUserByIdRequest.BuildRoute(999));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
