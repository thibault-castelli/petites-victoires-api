using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Api.Users;
using PetitesVictoires.Api.Users.Create;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Users;

[TestFixture]
public class CreateUserEndpointTests : FunctionalTestBase
{
    private static readonly object ValidUser = new
    {
        emailAddress = "new@mail.com", name = "newuser", password = "Password123!"
    };

    [Test]
    public async Task CreateUser_WhenValid_Returns201()
    {
        var response = await CreateClient().PostAsJsonAsync(CreateUserRequest.Route, ValidUser);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Test]
    public async Task CreateUser_WhenValid_ReturnsCreatedUser()
    {
        var response = await CreateClient().PostAsJsonAsync(CreateUserRequest.Route, ValidUser);

        var created = await response.Content.ReadFromJsonAsync<UserRecord>();
        created!.EmailAddress.ShouldBe("new@mail.com");
        created.Name.ShouldBe("newuser");
    }

    [Test]
    public async Task CreateUser_WhenValid_PersistsDomainUser()
    {
        await CreateClient().PostAsJsonAsync(CreateUserRequest.Route, ValidUser);

        await using var verify = DatabaseFixture.CreateContext();
        var user = await verify.Users.SingleAsync();
        user.EmailAddress.Value.ShouldBe("new@mail.com");
    }

    [Test]
    public async Task CreateUser_WhenValid_PersistsIdentityUser()
    {
        await CreateClient().PostAsJsonAsync(CreateUserRequest.Route, ValidUser);

        await using var verify = DatabaseFixture.CreateContext();
        (await verify.Set<Infrastructure.Identity.ApplicationUser>().CountAsync()).ShouldBe(1);
    }

    [Test]
    public async Task CreateUser_WhenEmailMalformed_Returns400()
    {
        var response = await CreateClient().PostAsJsonAsync(CreateUserRequest.Route,
            new { emailAddress = "not-an-email", name = "newuser", password = "Password123!" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateUser_WhenPasswordTooWeak_Returns400()
    {
        var response = await CreateClient().PostAsJsonAsync(CreateUserRequest.Route,
            new { emailAddress = "weak@mail.com", name = "weakuser", password = "weak" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateUser_WhenEmailAlreadyRegistered_Returns400()
    {
        await CreateClient().PostAsJsonAsync(CreateUserRequest.Route, ValidUser);

        var response = await CreateClient().PostAsJsonAsync(CreateUserRequest.Route,
            new { emailAddress = "new@mail.com", name = "different", password = "Password123!" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
