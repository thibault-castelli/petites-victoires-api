using PetitesVictoires.Core.Common;
using Shouldly;
using Vogen;

namespace PetitesVictoires.UnitTests.Core.Common;

[TestFixture]
public class EmailTests
{
    [TestCase("user@example.com")]
    [TestCase("a.b-c@sub.domain.co")]
    public void From_ValidAddress_ExposesTheValue(string address)
    {
        Email.From(address).Value.ShouldBe(address);
    }

    [Test]
    public void From_TrimsSurroundingWhitespace()
    {
        Email.From("  user@example.com  ").Value.ShouldBe("user@example.com");
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("not-an-email")]
    [TestCase("missing@")]
    [TestCase("@no-local.com")]
    [TestCase("two@@example.com")]
    public void From_InvalidAddress_Throws(string address)
    {
        Should.Throw<ValueObjectValidationException>(() => Email.From(address));
    }

    [Test]
    public void From_AddressLongerThanMaxLength_Throws()
    {
        var tooLong = new string('a', Email.MaxLength) + "@example.com";

        Should.Throw<ValueObjectValidationException>(() => Email.From(tooLong));
    }

    [Test]
    public void From_DisplayNameFormat_Throws_BecauseParsedAddressDiffersFromInput()
    {
        Should.Throw<ValueObjectValidationException>(() => Email.From("User <user@example.com>"));
    }

    [Test]
    public void TryFrom_InvalidAddress_ReturnsFalse()
    {
        Email.TryFrom("nope", out _).ShouldBeFalse();
    }
}
