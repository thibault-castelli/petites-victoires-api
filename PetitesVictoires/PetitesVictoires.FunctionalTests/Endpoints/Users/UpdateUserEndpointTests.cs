using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Api.Users;
using PetitesVictoires.Api.Users.Update;
using PetitesVictoires.Core.UserAggregate;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Users;

[TestFixture]
public class UpdateUserEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task UpdateUser_WhenAnonymous_Returns401()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        var response = await CreateClient().PutAsJsonAsync(UpdateUserRequest.BuildRoute(userId),
            new { emailAddress = "alice@mail.com", name = "renamed" });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task UpdateUser_WhenNotSelf_Returns403()
    {
        var aliceId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");
        var bobId = await RegisterUserAsync("bob@mail.com", "bob", "Password123!");

        var response = await CreateClientAs(bobId).PutAsJsonAsync(UpdateUserRequest.BuildRoute(aliceId),
            new { emailAddress = "hijacked@mail.com", name = "hijacked" });

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task UpdateUser_WhenSelf_Returns200()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        var response = await CreateClientAs(userId).PutAsJsonAsync(UpdateUserRequest.BuildRoute(userId),
            new { emailAddress = "alice.new@mail.com", name = "alice.renamed" });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task UpdateUser_WhenSelf_ReturnsUpdatedUser()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        var response = await CreateClientAs(userId).PutAsJsonAsync(UpdateUserRequest.BuildRoute(userId),
            new { emailAddress = "alice.new@mail.com", name = "alice.renamed" });

        var updated = await response.Content.ReadFromJsonAsync<UserRecord>();
        updated!.EmailAddress.ShouldBe("alice.new@mail.com");
        updated.Name.ShouldBe("alice.renamed");
    }

    [Test]
    public async Task UpdateUser_WhenSelf_PersistsChanges()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        await CreateClientAs(userId).PutAsJsonAsync(UpdateUserRequest.BuildRoute(userId),
            new { emailAddress = "alice.new@mail.com", name = "alice.renamed" });

        await using var verify = DatabaseFixture.CreateContext();
        var user = await verify.Users.FirstAsync(u => u.Id == UserId.From(userId));
        user.EmailAddress.Value.ShouldBe("alice.new@mail.com");
    }

    [Test]
    public async Task UpdateUser_WhenNameEmpty_Returns400()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        var response = await CreateClientAs(userId).PutAsJsonAsync(UpdateUserRequest.BuildRoute(userId),
            new { emailAddress = "alice@mail.com", name = "" });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateUser_WhenChangingPassword_AllowsSignInWithNewPassword()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        await CreateClientAs(userId).PutAsJsonAsync(UpdateUserRequest.BuildRoute(userId),
            new
            {
                emailAddress = "alice@mail.com",
                name = "alice",
                currentPassword = "Password123!",
                newPassword = "NewPassword456!"
            });

        var signIn = await CreateClient().PostAsJsonAsync("/Users/Sign-In",
            new { emailAddress = "alice@mail.com", password = "NewPassword456!" });
        signIn.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
