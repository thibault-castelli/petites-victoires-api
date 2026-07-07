using PetitesVictoires.Core.UserAggregate;
using Shouldly;
using Vogen;

namespace PetitesVictoires.UnitTests.Core.UserAggregate;

[TestFixture]
public class UserNameTests
{
    [Test]
    public void From_ValidName_ExposesTheValue()
    {
        UserName.From("Thibault").Value.ShouldBe("Thibault");
    }

    [Test]
    public void From_TrimsSurroundingWhitespace()
    {
        UserName.From("  Thibault  ").Value.ShouldBe("Thibault");
    }

    [Test]
    public void From_NameAtMaxLength_IsValid()
    {
        var atLimit = new string('a', UserName.MaxLength);

        UserName.From(atLimit).Value.Length.ShouldBe(UserName.MaxLength);
    }

    [TestCase("")]
    [TestCase("   ")]
    public void From_EmptyOrWhitespaceOnly_Throws(string name)
    {
        Should.Throw<ValueObjectValidationException>(() => UserName.From(name));
    }

    [Test]
    public void From_NameLongerThanMaxLength_Throws()
    {
        var tooLong = new string('a', UserName.MaxLength + 1);

        Should.Throw<ValueObjectValidationException>(() => UserName.From(tooLong));
    }

    [Test]
    public void TryFrom_InvalidName_ReturnsFalse()
    {
        UserName.TryFrom("", out _).ShouldBeFalse();
    }
}
