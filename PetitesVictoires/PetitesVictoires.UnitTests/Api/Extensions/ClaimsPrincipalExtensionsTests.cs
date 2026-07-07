using System.Security.Claims;
using PetitesVictoires.Api.Extensions;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Extensions;

[TestFixture]
public class ClaimsPrincipalExtensionsTests
{
    private static ClaimsPrincipal PrincipalWith(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    [Test]
    public void TryGetAuthenticatedUserId_WithValidNameIdentifier_ReturnsTrueAndParsesId()
    {
        var principal = PrincipalWith(new Claim(ClaimTypes.NameIdentifier, "42"));

        var success = principal.TryGetAuthenticatedUserId(out var id);

        success.ShouldBeTrue();
        id.ShouldBe(42);
    }

    [Test]
    public void TryGetAuthenticatedUserId_WithNoClaim_ReturnsFalse()
    {
        var principal = PrincipalWith();

        principal.TryGetAuthenticatedUserId(out var id).ShouldBeFalse();
        id.ShouldBe(0);
    }

    [Test]
    public void TryGetAuthenticatedUserId_WithNonNumericClaim_ReturnsFalse()
    {
        var principal = PrincipalWith(new Claim(ClaimTypes.NameIdentifier, "not-a-number"));

        principal.TryGetAuthenticatedUserId(out _).ShouldBeFalse();
    }

    [Test]
    public void GetAuthenticatedUserId_WithValidClaim_ReturnsId()
    {
        var principal = PrincipalWith(new Claim(ClaimTypes.NameIdentifier, "7"));

        principal.GetAuthenticatedUserId().ShouldBe(7);
    }

    [Test]
    public void GetAuthenticatedUserId_WithoutValidClaim_Throws()
    {
        var principal = PrincipalWith();

        Should.Throw<InvalidOperationException>(() => principal.GetAuthenticatedUserId());
    }
}
