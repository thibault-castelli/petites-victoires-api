using FastEndpoints;
using FluentValidation;

namespace PetitesVictoires.Api.Users.SignIn;

public class SignInValidator : Validator<SignInRequest>
{
    public SignInValidator()
    {
        RuleFor(r => r.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address is required")
            .EmailAddress();

        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
