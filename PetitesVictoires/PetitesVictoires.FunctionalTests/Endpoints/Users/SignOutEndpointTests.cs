using System.Net;
using System.Net.Http.Json;
using PetitesVictoires.Api.Posts.Create;
using PetitesVictoires.Api.Users.SignIn;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Users;

[TestFixture]
public class SignOutEndpointTests : FunctionalTestBase
{
    private const string SignOutRoute = "/Users/Sign-Out";

    [Test]
    public async Task SignOut_WhenAnonymous_Returns401()
    {
        var response = await CreateClient().PostAsync(SignOutRoute, null);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task SignOut_WhenSignedIn_Returns204()
    {
        await RegisterUserAsync("alice@mail.com", "alice", "Password123!");
        var client = CreateClient();
        await client.PostAsJsonAsync(SignInRequest.Route,
            new { emailAddress = "alice@mail.com", password = "Password123!" });

        var response = await client.PostAsync(SignOutRoute, null);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task SignOut_ThenAuthenticatedRequest_IsRejected()
    {
        await RegisterUserAsync("alice@mail.com", "alice", "Password123!");
        var client = CreateClient();
        await client.PostAsJsonAsync(SignInRequest.Route,
            new { emailAddress = "alice@mail.com", password = "Password123!" });
        await client.PostAsync(SignOutRoute, null);

        var response = await client.PostAsJsonAsync(CreatePostRequest.Route, new { content = "should fail" });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
