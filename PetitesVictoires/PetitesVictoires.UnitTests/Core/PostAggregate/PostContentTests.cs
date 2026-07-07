using PetitesVictoires.Core.PostAggregate;
using Shouldly;
using Vogen;

namespace PetitesVictoires.UnitTests.Core.PostAggregate;

[TestFixture]
public class PostContentTests
{
    [Test]
    public void From_ValidContent_ExposesTheValue()
    {
        var content = PostContent.From("A small victory");

        content.Value.ShouldBe("A small victory");
    }

    [Test]
    public void From_TrimsSurroundingWhitespace()
    {
        var content = PostContent.From("  padded content  ");

        content.Value.ShouldBe("padded content");
    }

    [Test]
    public void From_ContentAtMaxLength_IsValid()
    {
        var atLimit = new string('a', PostContent.MaxLength);

        var content = PostContent.From(atLimit);

        content.Value.Length.ShouldBe(PostContent.MaxLength);
    }

    [Test]
    public void From_EmptyString_Throws()
    {
        Should.Throw<ValueObjectValidationException>(() => PostContent.From(""));
    }

    [Test]
    public void From_WhitespaceOnly_Throws_BecauseItNormalizesToEmpty()
    {
        Should.Throw<ValueObjectValidationException>(() => PostContent.From("   "));
    }

    [Test]
    public void From_ContentLongerThanMaxLength_Throws()
    {
        var tooLong = new string('a', PostContent.MaxLength + 1);

        Should.Throw<ValueObjectValidationException>(() => PostContent.From(tooLong));
    }

    [Test]
    public void TryFrom_InvalidContent_ReturnsFalse()
    {
        var succeeded = PostContent.TryFrom("", out _);

        succeeded.ShouldBeFalse();
    }

    [Test]
    public void Equality_IsBasedOnTheNormalizedValue()
    {
        var a = PostContent.From("hello");
        var b = PostContent.From("  hello  ");

        a.ShouldBe(b);
    }
}
