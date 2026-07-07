using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Core.UserAggregate.Specifications;
using Shouldly;

namespace PetitesVictoires.UnitTests.Core.UserAggregate.Specifications;

[TestFixture]
public class UserByIdSpecificationTests
{
    private static User UserWith(int id)
    {
        return new User(UserId.From(id), Email.From("user@example.com"), UserName.From("name"));
    }

    [Test]
    public void Evaluate_ReturnsOnlyTheUserWithTheMatchingId()
    {
        var users = new[] { UserWith(1), UserWith(2), UserWith(3) };

        var results = new UserByIdSpecification(UserId.From(2)).Evaluate(users).ToList();

        results.ShouldHaveSingleItem().Id.ShouldBe(UserId.From(2));
    }

    [Test]
    public void Evaluate_WhenNoUserMatches_ReturnsEmpty()
    {
        var users = new[] { UserWith(1), UserWith(2) };

        var results = new UserByIdSpecification(UserId.From(99)).Evaluate(users);

        results.ShouldBeEmpty();
    }
}
