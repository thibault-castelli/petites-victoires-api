using PetitesVictoires.Api.Posts.List;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts;
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
            Email.From("user@example.com"), UserName.From("user"), createdAt);
        // Distinct paging values so a swapped field would fail.
        var paged = new PagedResult<PostDto>([dto], 2, 20, 41, 3);

        var response = new ListPostsMapper().FromEntity(paged);

        var item = response.Items.ShouldHaveSingleItem();
        item.Id.ShouldBe(7);
        item.Content.ShouldBe("content");
        item.UserId.ShouldBe(2);
        item.UserEmailAddress.ShouldBe("user@example.com");
        item.UserName.ShouldBe("user");
        item.CreatedAt.ShouldBe(createdAt);

        response.Page.ShouldBe(2);
        response.CountPerPage.ShouldBe(20);
        response.TotalEntityCount.ShouldBe(41);
        response.TotalPages.ShouldBe(3);
    }
}
