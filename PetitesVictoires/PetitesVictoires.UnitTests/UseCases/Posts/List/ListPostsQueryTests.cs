using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Common;
using PetitesVictoires.UseCases.Posts.List;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Posts.List;

[TestFixture]
public class ListPostsQueryTests
{
    private static ListPostsQuery Query(
        int? page = 1,
        int? countPerPage = Constants.DefaultPageSize,
        PostSortBy sortBy = PostSortBy.CreatedAt,
        int? likedBy = null,
        int? createdBy = null)
    {
        return new ListPostsQuery(
            new ListQueryParams(page, countPerPage),
            new ListPostsCriteria(
                sortBy,
                likedBy is null ? null : UserId.From(likedBy.Value),
                createdBy is null ? null : UserId.From(createdBy.Value)));
    }

    [Test]
    public void CacheKey_ForDefaultUnfilteredFirstPage_IsStableKey()
    {
        Query().CacheKey.ShouldBe($"posts:list:sort{PostSortBy.CreatedAt}:page:1");
    }

    [Test]
    public void CacheKey_IncludesSortBy()
    {
        Query(sortBy: PostSortBy.LikesCount).CacheKey
            .ShouldBe($"posts:list:sort{PostSortBy.LikesCount}:page:1");
    }

    [Test]
    public void CacheKey_WhenPageOmitted_NormalizesToPageOne()
    {
        Query(page: null).CacheKey.ShouldBe(Query(page: 1).CacheKey);
    }

    [Test]
    public void CacheKey_WhenCountPerPageOmitted_IsCached()
    {
        Query(countPerPage: null).CacheKey.ShouldNotBeEmpty();
    }

    [Test]
    public void CacheKey_AtMaxCachedPage_IsCached()
    {
        Query(page: Constants.MaxCachedPage).CacheKey.ShouldNotBeEmpty();
    }

    [Test]
    public void CacheKey_BeyondMaxCachedPage_IsEmpty()
    {
        Query(page: Constants.MaxCachedPage + 1).CacheKey.ShouldBeEmpty();
    }

    [Test]
    public void CacheKey_WhenLikedByPresent_IsEmpty()
    {
        Query(likedBy: 5).CacheKey.ShouldBeEmpty();
    }

    [Test]
    public void CacheKey_WhenCreatedByPresent_IsEmpty()
    {
        Query(createdBy: 5).CacheKey.ShouldBeEmpty();
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
