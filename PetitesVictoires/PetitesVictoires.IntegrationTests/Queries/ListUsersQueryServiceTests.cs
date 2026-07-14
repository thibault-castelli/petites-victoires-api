using PetitesVictoires.Infrastructure.Queries;
using PetitesVictoires.UseCases.Common;
using PetitesVictoires.UseCases.Users.List;
using Shouldly;

namespace PetitesVictoires.IntegrationTests.Queries;

[TestFixture]
public class ListUsersQueryServiceTests : IntegrationTestBase
{
    private ListUsersQueryService _sut = null!;

    [SetUp]
    public void CreateSut() => _sut = new ListUsersQueryService(DbContext);

    private static ListQueryParams Paging(int page = 1, int countPerPage = 10) => new(page, countPerPage);
    private static ListUsersCriteria NoSearch => new ListUsersCriteria(null);
    private static ListUsersCriteria Search(string term) => new(term);

    [Test]
    public async Task ListAsync_WhenNoSearch_OrdersByIdAscending()
    {
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(3, "c@mail.com", "carol");

        var result = await _sut.ListAsync(Paging(), NoSearch, CancellationToken.None);

        result.Items.Select(u => u.Id.Value).ShouldBe([1, 2, 3]);
    }

    [Test]
    public async Task ListAsync_WhenNoSearch_TotalEntityCountEqualsUserCount()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");

        var result = await _sut.ListAsync(Paging(), NoSearch, CancellationToken.None);

        result.TotalEntityCount.ShouldBe(2);
    }

    [Test]
    public async Task ListAsync_SecondPage_SkipsFirstPageUsers()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedUserAsync(3, "c@mail.com", "carol");

        var result = await _sut.ListAsync(Paging(page: 2, countPerPage: 2), NoSearch, CancellationToken.None);

        result.Items.Select(u => u.Id.Value).ShouldBe([3]);
    }

    [Test]
    public async Task ListAsync_MapsUserFields()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");

        var result = await _sut.ListAsync(Paging(), NoSearch, CancellationToken.None);

        var dto = result.Items.Single();
        dto.Id.Value.ShouldBe(1);
        dto.EmailAddress.Value.ShouldBe("alice@mail.com");
        dto.Name.Value.ShouldBe("alice");
    }

    [Test]
    public async Task ListAsync_WhenSearch_ReturnsUsersMatchingNameOrEmail()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "bob@mail.com", "bob");

        var result = await _sut.ListAsync(Paging(), Search("bob"), CancellationToken.None);

        result.Items.Select(u => u.Name.Value).ShouldBe(["bob"]);
    }

    [Test]
    public async Task ListAsync_WhenSearch_IsCaseInsensitive()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");

        var result = await _sut.ListAsync(Paging(), Search("ALICE"), CancellationToken.None);

        result.Items.ShouldHaveSingleItem().Name.Value.ShouldBe("alice");
    }

    [Test]
    public async Task ListAsync_WhenSearch_RanksExactThenPrefixThenContains()
    {
        await SeedUserAsync(1, "u1@mail.com", "contains-ann-inside");
        await SeedUserAsync(2, "u2@mail.com", "ann");
        await SeedUserAsync(3, "u3@mail.com", "annie");

        var result = await _sut.ListAsync(Paging(), Search("ann"), CancellationToken.None);

        result.Items.Select(u => u.Name.Value).ShouldBe(["ann", "annie", "contains-ann-inside"]);
    }

    [Test]
    public async Task ListAsync_WhenSearch_TotalEntityCountEqualsMatchCount()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "alicia@mail.com", "alicia");
        await SeedUserAsync(3, "bob@mail.com", "bob");

        var result = await _sut.ListAsync(Paging(), Search("ali"), CancellationToken.None);

        result.TotalEntityCount.ShouldBe(2);
    }

    [Test]
    public async Task ListAsync_WhenSearchHasNoMatches_ReturnsEmpty()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");

        var result = await _sut.ListAsync(Paging(), Search("zzz"), CancellationToken.None);

        result.Items.ShouldBeEmpty();
    }

    [Test]
    public async Task ListAsync_WhenSearchContainsWildcard_TreatsItLiterally()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        await SeedUserAsync(2, "percent@mail.com", "50%off");

        var result = await _sut.ListAsync(Paging(), Search("%"), CancellationToken.None);

        result.Items.Select(u => u.Name.Value).ShouldBe(["50%off"]);
    }
}
