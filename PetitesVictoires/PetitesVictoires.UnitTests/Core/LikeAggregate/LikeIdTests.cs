using PetitesVictoires.Core.LikeAggregate;
using Shouldly;
using Vogen;

namespace PetitesVictoires.UnitTests.Core.LikeAggregate;

[TestFixture]
public class LikeIdTests
{
    [Test]
    public void From_PositiveValue_ExposesTheValue()
    {
        LikeId.From(42).Value.ShouldBe(42);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void From_NonPositiveValue_Throws(int value)
    {
        Should.Throw<ValueObjectValidationException>(() => LikeId.From(value));
    }

    [Test]
    public void Equality_IsBasedOnTheValue()
    {
        LikeId.From(1).ShouldBe(LikeId.From(1));
    }
}
