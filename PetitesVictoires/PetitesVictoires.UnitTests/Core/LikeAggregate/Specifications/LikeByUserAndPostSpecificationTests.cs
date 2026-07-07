using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.LikeAggregate.Specifications;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using Shouldly;

namespace PetitesVictoires.UnitTests.Core.LikeAggregate.Specifications;

[TestFixture]
public class LikeByUserAndPostSpecificationTests
{
    private static Like LikeBy(int userId, int postId)
    {
        return new Like(UserId.From(userId), PostId.From(postId));
    }

    [Test]
    public void Evaluate_ReturnsTheLikeMatchingBothUserAndPost()
    {
        var likes = new[] { LikeBy(1, 1), LikeBy(7, 2), LikeBy(3, 3) };

        var results = new LikeByUserAndPostSpecification(UserId.From(7), PostId.From(2)).Evaluate(likes).ToList();

        var match = results.ShouldHaveSingleItem();
        match.UserId.ShouldBe(UserId.From(7));
        match.PostId.ShouldBe(PostId.From(2));
    }

    [Test]
    public void Evaluate_ExcludesLikeWithMatchingUserButDifferentPost()
    {
        var likes = new[] { LikeBy(7, 2) };

        var results = new LikeByUserAndPostSpecification(UserId.From(7), PostId.From(99)).Evaluate(likes);

        results.ShouldBeEmpty();
    }

    [Test]
    public void Evaluate_ExcludesLikeWithMatchingPostButDifferentUser()
    {
        var likes = new[] { LikeBy(7, 2) };

        var results = new LikeByUserAndPostSpecification(UserId.From(99), PostId.From(2)).Evaluate(likes);

        results.ShouldBeEmpty();
    }

    [Test]
    public void Evaluate_WhenNoLikeMatches_ReturnsEmpty()
    {
        var likes = new[] { LikeBy(1, 1), LikeBy(2, 2) };

        var results = new LikeByUserAndPostSpecification(UserId.From(5), PostId.From(5)).Evaluate(likes);

        results.ShouldBeEmpty();
    }
}
