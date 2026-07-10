using FluentValidation.TestHelper;
using PetitesVictoires.Api.Users.GetLikeStats;

namespace PetitesVictoires.UnitTests.Api.Users.GetLikeStats;

[TestFixture]
public class GetUserLikeStatsValidatorTests
{
    private readonly GetUserLikeStatsValidator _validator = new();

    [Test]
    public void PositiveUserId_HasNoErrors()
    {
        _validator.TestValidate(new GetUserLikeStatsRequest { UserId = 1 }).ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositiveUserId_HasError(int userId)
    {
        _validator.TestValidate(new GetUserLikeStatsRequest { UserId = userId })
            .ShouldHaveValidationErrorFor(r => r.UserId);
    }
}
