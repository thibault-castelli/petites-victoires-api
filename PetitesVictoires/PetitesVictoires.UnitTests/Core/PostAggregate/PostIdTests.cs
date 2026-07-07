using PetitesVictoires.Core.PostAggregate;
using Shouldly;
using Vogen;

namespace PetitesVictoires.UnitTests.Core.PostAggregate;

[TestFixture]
public class PostIdTests
{
    [Test]
    public void From_PositiveValue_ExposesTheValue()
    {
        var id = PostId.From(42);

        id.Value.ShouldBe(42);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void From_NonPositiveValue_Throws(int value)
    {
        Should.Throw<ValueObjectValidationException>(() => PostId.From(value));
    }

    [Test]
    public void Equality_IsBasedOnTheValue()
    {
        PostId.From(1).ShouldBe(PostId.From(1));
    }
}
