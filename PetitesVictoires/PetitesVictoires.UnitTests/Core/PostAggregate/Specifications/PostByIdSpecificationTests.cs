using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.PostAggregate.Specifications;
using PetitesVictoires.Core.UserAggregate;
using Shouldly;

namespace PetitesVictoires.UnitTests.Core.PostAggregate.Specifications;

[TestFixture]
public class PostByIdSpecificationTests
{
    private static Post PostWith(int id, int userId = 1)
    {
        return new Post(PostContent.From("content"), UserId.From(userId)) { Id = PostId.From(id) };
    }

    [Test]
    public void Evaluate_ReturnsOnlyThePostWithTheMatchingId()
    {
        var posts = new[] { PostWith(1), PostWith(2), PostWith(3) };

        var results = new PostByIdSpecification(PostId.From(2)).Evaluate(posts).ToList();

        results.ShouldHaveSingleItem().Id.ShouldBe(PostId.From(2));
    }

    [Test]
    public void Evaluate_WhenNoPostMatches_ReturnsEmpty()
    {
        var posts = new[] { PostWith(1), PostWith(2) };

        var results = new PostByIdSpecification(PostId.From(99)).Evaluate(posts);

        results.ShouldBeEmpty();
    }
}
