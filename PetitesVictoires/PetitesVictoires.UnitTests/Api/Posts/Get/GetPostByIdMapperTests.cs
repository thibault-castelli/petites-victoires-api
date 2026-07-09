using PetitesVictoires.Api.Posts.Get;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Posts;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Posts.Get;

[TestFixture]
public class GetPostByIdMapperTests
{
    [Test]
    public void FromEntity_UnwrapsEachValueObjectIntoTheMatchingRecordField()
    {
        var createdAt = DateTime.UtcNow;
        var dto = new PostDto(PostId.From(1), PostContent.From("content"), UserId.From(2),
            Email.From("user@example.com"), UserName.From("user"), createdAt, 1);

        var record = new GetPostByIdMapper().FromEntity(dto);

        record.Id.ShouldBe(1);
        record.Content.ShouldBe("content");
        record.UserId.ShouldBe(2);
        record.UserEmailAddress.ShouldBe("user@example.com");
        record.UserName.ShouldBe("user");
        record.LikesCount.ShouldBe(1);
        record.CreatedAt.ShouldBe(createdAt);
    }
}
