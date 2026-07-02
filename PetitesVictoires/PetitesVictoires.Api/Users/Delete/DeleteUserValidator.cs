using FastEndpoints;
using FluentValidation;

namespace PetitesVictoires.Api.Users.Delete;

public sealed class DeleteUserValidator : Validator<DeleteUserRequest>
{
    public DeleteUserValidator()
    {
        RuleFor(request => request.UserId)
            .GreaterThan(0);
    }
}
