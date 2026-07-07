using FluentValidation.TestHelper;
using PetitesVictoires.Api.Users.Get;

namespace PetitesVictoires.UnitTests.Api.Users.Get;

[TestFixture]
public class GetUserByIdValidatorTests
{
    private readonly GetUserByIdValidator _validator = new();

    [Test]
    public void PositiveUserId_HasNoErrors()
    {
        _validator.TestValidate(new GetUserByIdRequest { UserId = 1 }).ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositiveUserId_HasError(int userId)
    {
        _validator.TestValidate(new GetUserByIdRequest { UserId = userId })
            .ShouldHaveValidationErrorFor(r => r.UserId);
    }
}
