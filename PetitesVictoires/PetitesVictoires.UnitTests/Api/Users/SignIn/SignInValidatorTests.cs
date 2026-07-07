using FluentValidation.TestHelper;
using PetitesVictoires.Api.Users.SignIn;

namespace PetitesVictoires.UnitTests.Api.Users.SignIn;

[TestFixture]
public class SignInValidatorTests
{
    private readonly SignInValidator _validator = new();

    private static SignInRequest Request(string email = "user@example.com", string password = "secret")
    {
        return new SignInRequest { EmailAddress = email, Password = password };
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
    public void EmptyPassword_HasRequiredMessage()
    {
        _validator.TestValidate(Request(password: ""))
            .ShouldHaveValidationErrorFor(r => r.Password)
            .WithErrorMessage("Password is required");
    }
}
