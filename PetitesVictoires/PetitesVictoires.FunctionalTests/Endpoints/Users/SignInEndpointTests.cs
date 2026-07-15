using System.Net;
using System.Net.Http.Json;
using PetitesVictoires.Api.Posts.Create;
using PetitesVictoires.Api.Users.SignIn;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Users;

[TestFixture]
public class SignInEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task SignIn_WithValidCredentials_Returns204()
    {
        await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        var response = await CreateClient().PostAsJsonAsync(SignInRequest.Route,
            new { emailAddress = "alice@mail.com", password = "Password123!" });

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task SignIn_WithValidCredentials_IssuesAuthCookie()
    {
        await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        var response = await CreateClient().PostAsJsonAsync(SignInRequest.Route,
            new { emailAddress = "alice@mail.com", password = "Password123!" });

        response.Headers.Contains("Set-Cookie").ShouldBeTrue();
    }

    [Test]
    public async Task SignIn_WithWrongPassword_Returns401()
    {
        await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        var response = await CreateClient().PostAsJsonAsync(SignInRequest.Route,
            new { emailAddress = "alice@mail.com", password = "WrongPassword1!" });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task SignIn_WithUnknownEmail_Returns401()
    {
        var response = await CreateClient().PostAsJsonAsync(SignInRequest.Route,
            new { emailAddress = "nobody@mail.com", password = "Password123!" });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task SignIn_WhenPasswordMissing_Returns400()
    {
        var response = await CreateClient().PostAsJsonAsync(SignInRequest.Route,
            new { emailAddress = "alice@mail.com", password = "" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    // The cookie issued by the real sign-in must authenticate a later request: the test auth handler
    // defers to the cookie scheme when no X-Test-UserId header is present.
    [Test]
    public async Task SignIn_ThenCreatePost_AuthenticatesViaCookie()
    {
        await RegisterUserAsync("alice@mail.com", "alice", "Password123!");
        var client = CreateClient();
        await client.PostAsJsonAsync(SignInRequest.Route,
            new { emailAddress = "alice@mail.com", password = "Password123!" });

        var response = await client.PostAsJsonAsync(CreatePostRequest.Route, new { content = "signed in" });

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
