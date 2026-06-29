using FastEndpoints;
using FluentValidation;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Api.Users.CreateUser;

public class CreateUserValidator : Validator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(r => r.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address is required")
            .EmailAddress();

        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(UserName.MaxLength);

        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
