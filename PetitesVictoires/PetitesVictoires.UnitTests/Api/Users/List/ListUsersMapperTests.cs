using PetitesVictoires.Api.Users.List;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Users;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Users.List;

[TestFixture]
public class ListUsersMapperTests
{
    [Test]
    public void FromEntity_MapsEachItemAndPreservesPagingFields()
    {
        var createdAt = DateTime.UtcNow;
        var dto = new UserDto(UserId.From(9), Email.From("user@example.com"), UserName.From("user"), createdAt);
        // Distinct paging values so a swapped field would fail.
        var paged = new PagedResult<UserDto>([dto], 2, 20, 41, 3);

        var response = new ListUsersMapper().FromEntity(paged);

        var item = response.Items.ShouldHaveSingleItem();
        item.Id.ShouldBe(9);
        item.EmailAddress.ShouldBe("user@example.com");
        item.Name.ShouldBe("user");
        item.CreatedAt.ShouldBe(createdAt);

        response.Page.ShouldBe(2);
        response.CountPerPage.ShouldBe(20);
        response.TotalEntityCount.ShouldBe(41);
        response.TotalPages.ShouldBe(3);
    }
}
