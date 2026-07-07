using PetitesVictoires.Api.Users.Update;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Users.Update;

[TestFixture]
public class UpdateUserMapperTests
{
    [Test]
    public void FromEntity_UnwrapsEachValueObjectIntoTheMatchingRecordField()
    {
        var createdAt = DateTime.UtcNow;
        var dto = new UserDto(UserId.From(1), Email.From("user@example.com"), UserName.From("user"), createdAt);

        var record = new UpdateUserMapper().FromEntity(dto);

        record.Id.ShouldBe(1);
        record.EmailAddress.ShouldBe("user@example.com");
        record.Name.ShouldBe("user");
        record.CreatedAt.ShouldBe(createdAt);
    }
}
