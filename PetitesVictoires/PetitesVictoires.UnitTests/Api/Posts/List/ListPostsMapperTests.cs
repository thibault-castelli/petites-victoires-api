using PetitesVictoires.Api.Posts.List;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts;
using PetitesVictoires.UseCases.Posts.List;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Posts.List;

[TestFixture]
public class ListPostsMapperTests
{
    [Test]
    public void FromEntity_MapsEachItemAndPreservesPagingFields()
    {
        var createdAt = DateTime.UtcNow;
        var dto = new PostDto(PostId.From(7), PostContent.From("content"), UserId.From(2),
            Email.From("user@example.com"), UserName.From("user"), createdAt, 1);
        // Distinct paging values so a swapped field would fail.
        var paged = new PagedResult<PostDto>([dto], 2, 20, 41, 3);

        var response = new ListPostsMapper().FromEntity(paged);

        var item = response.Items.ShouldHaveSingleItem();
        item.Id.ShouldBe(7);
        item.Content.ShouldBe("content");
        item.UserId.ShouldBe(2);
        item.UserEmailAddress.ShouldBe("user@example.com");
        item.UserName.ShouldBe("user");
        item.LikesCount.ShouldBe(1);
        item.CreatedAt.ShouldBe(createdAt);

        response.Page.ShouldBe(2);
        response.CountPerPage.ShouldBe(20);
        response.TotalEntityCount.ShouldBe(41);
        response.TotalPages.ShouldBe(3);
    }

    [Test]
    public void ToQuery_MapsPagingIntoQueryParams()
    {
        var request = new ListPostsRequest { Page = 3, CountPerPage = 25 };

        var query = new ListPostsMapper().ToQuery(request);

        query.ListQueryParams.Page.ShouldBe(3);
        query.ListQueryParams.CountPerPage.ShouldBe(25);
    }

    [TestCase("created_at", PostSortBy.CreatedAt)]
    [TestCase("likes_count", PostSortBy.LikesCount)]
    [TestCase(null, PostSortBy.CreatedAt)]
    [TestCase("unknown", PostSortBy.CreatedAt)]
    public void ToQuery_MapsSortByStringToEnum(string? sortBy, PostSortBy expected)
    {
        var request = new ListPostsRequest { SortBy = sortBy };

        var query = new ListPostsMapper().ToQuery(request);

        query.ListPostsCriteria.SortBy.ShouldBe(expected);
    }

    [Test]
    public void ToQuery_MapsUserIdFilters()
    {
        var request = new ListPostsRequest { LikedBy = 4, CreatedBy = 9 };

        var query = new ListPostsMapper().ToQuery(request);

        query.ListPostsCriteria.LikedBy.ShouldBe(UserId.From(4));
        query.ListPostsCriteria.CreatedBy.ShouldBe(UserId.From(9));
    }

    [Test]
    public void ToQuery_WhenFiltersAreNull_LeavesThemNull()
    {
        var request = new ListPostsRequest { LikedBy = null, CreatedBy = null };

        var query = new ListPostsMapper().ToQuery(request);

        query.ListPostsCriteria.LikedBy.ShouldBeNull();
        query.ListPostsCriteria.CreatedBy.ShouldBeNull();
    }
}
