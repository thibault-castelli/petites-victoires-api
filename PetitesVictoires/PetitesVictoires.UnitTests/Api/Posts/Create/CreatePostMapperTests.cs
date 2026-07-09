using PetitesVictoires.Api.Posts.Create;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Posts;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Posts.Create;

[TestFixture]
public class CreatePostMapperTests
{
    [Test]
    public void FromEntity_UnwrapsEachValueObjectIntoTheMatchingRecordField()
    {
        var createdAt = DateTime.UtcNow;
        var dto = new PostDto(PostId.From(1), PostContent.From("content"), UserId.From(2),
            Email.From("user@example.com"), UserName.From("user"), createdAt);

        var record = new CreatePostMapper().FromEntity(dto);

        record.Id.ShouldBe(1);
        record.Content.ShouldBe("content");
        record.UserId.ShouldBe(2);
        record.UserEmailAddress.ShouldBe("user@example.com");
        record.UserName.ShouldBe("user");
        record.LikesCount.ShouldBe(0);
        record.CreatedAt.ShouldBe(createdAt);
    }
}
