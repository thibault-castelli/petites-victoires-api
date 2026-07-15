using System.Net;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Api.Users.Delete;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Users;

[TestFixture]
public class DeleteUserEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task DeleteUser_WhenAnonymous_Returns401()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        var response = await CreateClient().DeleteAsync(DeleteUserRequest.BuildRoute(userId));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task DeleteUser_WhenNotSelf_Returns403()
    {
        var aliceId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");
        var bobId = await RegisterUserAsync("bob@mail.com", "bob", "Password123!");

        var response = await CreateClientAs(bobId).DeleteAsync(DeleteUserRequest.BuildRoute(aliceId));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task DeleteUser_WhenSelf_Returns204()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        var response = await CreateClientAs(userId).DeleteAsync(DeleteUserRequest.BuildRoute(userId));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task DeleteUser_WhenSelf_RemovesDomainUser()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        await CreateClientAs(userId).DeleteAsync(DeleteUserRequest.BuildRoute(userId));

        await using var verify = DatabaseFixture.CreateContext();
        (await verify.Users.AnyAsync()).ShouldBeFalse();
    }

    [Test]
    public async Task DeleteUser_WhenSelf_RemovesIdentityUser()
    {
        var userId = await RegisterUserAsync("alice@mail.com", "alice", "Password123!");

        await CreateClientAs(userId).DeleteAsync(DeleteUserRequest.BuildRoute(userId));

        await using var verify = DatabaseFixture.CreateContext();
        (await verify.Set<Infrastructure.Identity.ApplicationUser>().AnyAsync()).ShouldBeFalse();
    }
}
