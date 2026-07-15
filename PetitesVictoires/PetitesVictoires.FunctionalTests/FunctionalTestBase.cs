using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using PetitesVictoires.Api.Users;
using PetitesVictoires.Api.Users.Create;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.TestSupport;

namespace PetitesVictoires.FunctionalTests;

public abstract class FunctionalTestBase
{
    private static readonly Uri BaseAddress = new("https://localhost");

    /// <summary>
    ///     The auth cookie is issued with CookieSecurePolicy.Always, so HttpClient only sends it back
    ///     over https. TestServer does no real TLS — the scheme just has to say https.
    /// </summary>
    private static readonly WebApplicationFactoryClientOptions ClientOptions = new()
    {
        BaseAddress = new Uri("https://localhost")
    };

    protected PetitesVictoiresDbContext DbContext = null!;

    [SetUp]
    public async Task BaseSetUp()
    {
        await DatabaseFixture.ResetAsync();
        DbContext = DatabaseFixture.CreateContext();
    }

    [TearDown]
    public async Task BaseTearDown()
    {
        await DbContext.DisposeAsync();
    }

    protected static HttpClient CreateClient()
    {
        return DatabaseFixture.Factory.CreateDefaultClient(
            BaseAddress, new ApiRoutePrefixHandler(), new CookieContainerHandler());
    }

    protected static HttpClient CreateClientAs(int userId)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        return client;
    }

    protected Task SeedUserAsync(int id, string email = "user@mail.com", string name = "user")
    {
        return DbContext.SeedUserAsync(id, email, name);
    }

    protected Task SeedPostAsync(int id, string content, int userId, DateTime? createdAt = null)
    {
        return DbContext.SeedPostAsync(id, content, userId, createdAt);
    }

    protected Task SeedLikeAsync(int id, int userId, int postId)
    {
        return DbContext.SeedLikeAsync(id, userId, postId);
    }

    protected Task SoftDeletePostAsync(int id)
    {
        return DbContext.SoftDeletePostAsync(id);
    }

    /// <summary>
    ///     Registers a real Identity + domain user through the API. Endpoints that go through
    ///     IIdentityService (update/delete user, sign-in) need the AspNetUsers row, which raw seeding
    ///     does not create.
    /// </summary>
    protected static async Task<int> RegisterUserAsync(string email, string name, string password)
    {
        var response = await CreateClient()
            .PostAsJsonAsync(CreateUserRequest.Route, new { emailAddress = email, name, password });
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<UserRecord>();
        return created!.Id;
    }
}

/// <summary>Mirror of the paged list responses; avoids deserializing into the inherited PagedResult record.</summary>
public record PagedBody<T>(List<T> Items, int Page, int CountPerPage, int TotalEntityCount, int TotalPages);
