using FluentValidation.TestHelper;
using PetitesVictoires.Api.Users.Update;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UnitTests.Api.Users.Update;

[TestFixture]
public class UpdateUserValidatorTests
{
    private readonly UpdateUserValidator _validator = new();

    private static UpdateUserRequest Request(
        int userId = 1, string email = "user@example.com", string name = "user")
    {
        return new UpdateUserRequest { UserId = userId, EmailAddress = email, Name = name };
    }

    [Test]
    public void ValidRequest_HasNoErrors()
    {
        _validator.TestValidate(Request()).ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositiveUserId_HasError(int userId)
    {
        _validator.TestValidate(Request(userId)).ShouldHaveValidationErrorFor(r => r.UserId);
    }

    [Test]
    public void EmptyEmail_HasRequiredMessage()
    {
        _validator.TestValidate(Request(email: ""))
            .ShouldHaveValidationErrorFor(r => r.EmailAddress)
            .WithErrorMessage("Email address is required");
    }

    [Test]
    public void InvalidEmailFormat_HasError()
    {
        _validator.TestValidate(Request(email: "not-an-email")).ShouldHaveValidationErrorFor(r => r.EmailAddress);
    }

    [Test]
    public void EmailLongerThanMaxLength_HasError()
    {
        var longEmail = new string('a', Email.MaxLength) + "@example.com";

        _validator.TestValidate(Request(email: longEmail)).ShouldHaveValidationErrorFor(r => r.EmailAddress);
    }

    [Test]
    public void EmptyName_HasRequiredMessage()
    {
        _validator.TestValidate(Request(name: ""))
            .ShouldHaveValidationErrorFor(r => r.Name)
            .WithErrorMessage("Name is required");
    }

    [Test]
    public void NameAtMaxLength_HasNoError()
    {
        _validator.TestValidate(Request(name: new string('a', UserName.MaxLength)))
            .ShouldNotHaveValidationErrorFor(r => r.Name);
    }

    [Test]
    public void NameLongerThanMaxLength_HasError()
    {
        _validator.TestValidate(Request(name: new string('a', UserName.MaxLength + 1)))
            .ShouldHaveValidationErrorFor(r => r.Name);
    }
}