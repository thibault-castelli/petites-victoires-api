using System.Net;
using System.Net.Http.Json;
using PetitesVictoires.Api.Users;
using PetitesVictoires.Api.Users.List;
using Shouldly;

namespace PetitesVictoires.FunctionalTests.Endpoints.Users;

[TestFixture]
public class ListUsersEndpointTests : FunctionalTestBase
{
    [Test]
    public async Task ListUsers_WhenAnonymous_Returns200()
    {
        var response = await CreateClient().GetAsync(ListUsersRequest.Route);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Test]
    public async Task ListUsers_ReturnsTotalEntityCount()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");

        var body = await CreateClient().GetFromJsonAsync<PagedBody<UserRecord>>(ListUsersRequest.Route);

        body!.TotalEntityCount.ShouldBe(2);
    }

    [Test]
    public async Task ListUsers_OrdersByIdAscending()
    {
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedUserAsync(1, "alice@mail.com", "alice");

        var body = await CreateClient().GetFromJsonAsync<PagedBody<UserRecord>>(ListUsersRequest.Route);

        body!.Items.Select(u => u.Name).ShouldBe(["alice", "bob"]);
    }

    [Test]
    public async Task ListUsers_WhenSearching_ReturnsOnlyMatchingUsers()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");

        var body = await CreateClient()
            .GetFromJsonAsync<PagedBody<UserRecord>>($"{ListUsersRequest.Route}?search=bob");

        body!.Items.Single().Name.ShouldBe("bob");
    }

    [Test]
    public async Task ListUsers_WhenSearching_IsCaseInsensitive()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");

        var body = await CreateClient()
            .GetFromJsonAsync<PagedBody<UserRecord>>($"{ListUsersRequest.Route}?search=ALICE");

        body!.Items.Single().Name.ShouldBe("alice");
    }

    [Test]
    public async Task ListUsers_WhenSecondPageRequested_SkipsFirstPage()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");
        await SeedUserAsync(3, "carol@mail.com", "carol");

        var body = await CreateClient()
            .GetFromJsonAsync<PagedBody<UserRecord>>($"{ListUsersRequest.Route}?page=2&count_per_page=2");

        body!.Items.Single().Name.ShouldBe("carol");
    }
}
