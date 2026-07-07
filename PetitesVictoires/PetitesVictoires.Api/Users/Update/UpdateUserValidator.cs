using FastEndpoints;
using FluentValidation;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Api.Users.Update;

public sealed class UpdateUserValidator : Validator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(request => request.UserId)
            .GreaterThan(0);

        RuleFor(request => request.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address is required")
            .MaximumLength(Email.MaxLength)
            .EmailAddress();

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(UserName.MaxLength);
    }
}
