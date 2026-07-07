using FluentValidation.TestHelper;
using PetitesVictoires.Api.Users.Create;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UnitTests.Api.Users.Create;

[TestFixture]
public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator = new();

    private static CreateUserRequest Request(
        string email = "user@example.com", string name = "user", string password = "secret")
    {
        return new CreateUserRequest { EmailAddress = email, Name = name, Password = password };
    }

    [Test]
    public void ValidRequest_HasNoErrors()
    {
        _validator.TestValidate(Request()).ShouldNotHaveAnyValidationErrors();
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

    [Test]
    public void EmptyPassword_HasRequiredMessage()
    {
        _validator.TestValidate(Request(password: ""))
            .ShouldHaveValidationErrorFor(r => r.Password)
            .WithErrorMessage("Password is required");
    }
}
