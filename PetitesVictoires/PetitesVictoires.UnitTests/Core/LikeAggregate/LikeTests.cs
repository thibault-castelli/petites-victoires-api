using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using Shouldly;

namespace PetitesVictoires.UnitTests.Core.LikeAggregate;

[TestFixture]
public class LikeTests
{
    private static readonly UserId AuthorId = UserId.From(1);
    private static readonly PostId TargetPostId = PostId.From(1);

    [Test]
    public void Constructor_SetsUserAndPost()
    {
        var like = new Like(AuthorId, TargetPostId);

        like.UserId.ShouldBe(AuthorId);
        like.PostId.ShouldBe(TargetPostId);
    }

    [Test]
    public void Constructor_StampsCreatedAtWithUtcNow()
    {
        var before = DateTime.UtcNow;

        var like = new Like(AuthorId, TargetPostId);

        like.CreatedAt.ShouldBeInRange(before, DateTime.UtcNow);
    }
}
