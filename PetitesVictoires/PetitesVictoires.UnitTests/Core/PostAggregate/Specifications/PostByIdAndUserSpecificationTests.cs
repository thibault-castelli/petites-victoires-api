using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.PostAggregate.Specifications;
using PetitesVictoires.Core.UserAggregate;
using Shouldly;

namespace PetitesVictoires.UnitTests.Core.PostAggregate.Specifications;

[TestFixture]
public class PostByIdAndUserSpecificationTests
{
    private static Post PostWith(int id, int userId)
    {
        return new Post(PostContent.From("content"), UserId.From(userId)) { Id = PostId.From(id) };
    }

    [Test]
    public void Evaluate_ReturnsThePostMatchingBothIdAndUser()
    {
        var posts = new[] { PostWith(1, 1), PostWith(2, 7), PostWith(3, 3) };

        var results = new PostByIdAndUserSpecification(PostId.From(2), UserId.From(7)).Evaluate(posts).ToList();

        var match = results.ShouldHaveSingleItem();
        match.Id.ShouldBe(PostId.From(2));
        match.UserId.ShouldBe(UserId.From(7));
    }

    [Test]
    public void Evaluate_ExcludesPostWithMatchingIdButDifferentUser()
    {
        var posts = new[] { PostWith(2, 7) };

        var results = new PostByIdAndUserSpecification(PostId.From(2), UserId.From(99)).Evaluate(posts);

        results.ShouldBeEmpty();
    }

    [Test]
    public void Evaluate_ExcludesPostWithMatchingUserButDifferentId()
    {
        var posts = new[] { PostWith(2, 7) };

        var results = new PostByIdAndUserSpecification(PostId.From(99), UserId.From(7)).Evaluate(posts);

        results.ShouldBeEmpty();
    }

    [Test]
    public void Evaluate_WhenNoPostMatches_ReturnsEmpty()
    {
        var posts = new[] { PostWith(1, 1), PostWith(2, 2) };

        var results = new PostByIdAndUserSpecification(PostId.From(5), UserId.From(5)).Evaluate(posts);

        results.ShouldBeEmpty();
    }
}
