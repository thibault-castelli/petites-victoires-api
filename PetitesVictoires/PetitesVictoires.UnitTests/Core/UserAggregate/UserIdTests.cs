using PetitesVictoires.Core.UserAggregate;
using Shouldly;
using Vogen;

namespace PetitesVictoires.UnitTests.Core.UserAggregate;

[TestFixture]
public class UserIdTests
{
    [Test]
    public void From_PositiveValue_ExposesTheValue()
    {
        UserId.From(42).Value.ShouldBe(42);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void From_NonPositiveValue_Throws(int value)
    {
        Should.Throw<ValueObjectValidationException>(() => UserId.From(value));
    }

    [Test]
    public void Equality_IsBasedOnTheValue()
    {
        UserId.From(1).ShouldBe(UserId.From(1));
    }
}
