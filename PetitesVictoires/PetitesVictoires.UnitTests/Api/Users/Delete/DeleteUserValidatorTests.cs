using FluentValidation.TestHelper;
using PetitesVictoires.Api.Users.Delete;

namespace PetitesVictoires.UnitTests.Api.Users.Delete;

[TestFixture]
public class DeleteUserValidatorTests
{
    private readonly DeleteUserValidator _validator = new();

    [Test]
    public void PositiveUserId_HasNoErrors()
    {
        _validator.TestValidate(new DeleteUserRequest { UserId = 1 }).ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositiveUserId_HasError(int userId)
    {
        _validator.TestValidate(new DeleteUserRequest { UserId = userId })
            .ShouldHaveValidationErrorFor(r => r.UserId);
    }
}
