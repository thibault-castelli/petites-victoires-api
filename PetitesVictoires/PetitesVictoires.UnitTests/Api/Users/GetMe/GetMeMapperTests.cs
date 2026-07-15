using PetitesVictoires.Api.Users.GetMe;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Users.GetMe;

[TestFixture]
public class GetMeMapperTests
{
    [Test]
    public void FromEntity_UnwrapsEachValueObjectIntoTheMatchingRecordField()
    {
        var createdAt = DateTime.UtcNow;
        var dto = new UserDto(UserId.From(2), Email.From("bob@example.com"), UserName.From("bob"), createdAt);

        var record = new GetMeMapper().FromEntity(dto);

        record.Id.ShouldBe(2);
        record.EmailAddress.ShouldBe("bob@example.com");
        record.Name.ShouldBe("bob");
        record.CreatedAt.ShouldBe(createdAt);
    }
}
