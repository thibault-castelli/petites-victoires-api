using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Common;
using PetitesVictoires.UseCases.Users.List;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.List;

[TestFixture]
public class ListUsersQueryTests
{
    private static ListUsersQuery Query(
        int? page = 1,
        int? countPerPage = Constants.DefaultPageSize,
        string? search = null)
    {
        return new ListUsersQuery(
            new ListQueryParams(page, countPerPage),
            new ListUsersCriteria(search));
    }

    [Test]
    public void CacheKey_ForDefaultNoSearchFirstPage_IsStableKey()
    {
        Query().CacheKey.ShouldBe("users:list:p1");
    }

    [Test]
    public void CacheKey_WhenPageOmitted_NormalizesToPageOne()
    {
        Query(page: null).CacheKey.ShouldBe(Query(page: 1).CacheKey);
    }

    [Test]
    public void CacheKey_AtMaxCachedPage_IsCached()
    {
        Query(page: Constants.MaxCachedPage).CacheKey.ShouldBe($"users:list:p{Constants.MaxCachedPage}");
    }

    [Test]
    public void CacheKey_BeyondMaxCachedPage_IsEmpty()
    {
        Query(page: Constants.MaxCachedPage + 1).CacheKey.ShouldBeEmpty();
    }

    [Test]
    public void CacheKey_WhenSearchPresent_IsEmpty()
    {
        Query(search: "alice").CacheKey.ShouldBeEmpty();
    }

    [Test]
    public void CacheKey_WhenSearchIsWhitespace_IsTreatedAsNoSearch()
    {
        Query(search: "   ").CacheKey.ShouldBe(Query(search: null).CacheKey);
    }

    [Test]
    public void CacheKey_WhenCountPerPageNotDefault_IsEmpty()
    {
        Query(countPerPage: Constants.DefaultPageSize + 1).CacheKey.ShouldBeEmpty();
    }

    [Test]
    public void CacheTimeout_IsFifteenSeconds()
    {
        Query().CacheTimeout.ShouldBe(TimeSpan.FromSeconds(15));
    }
}
