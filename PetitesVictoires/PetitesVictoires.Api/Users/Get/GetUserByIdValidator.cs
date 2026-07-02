using FastEndpoints;
using FluentValidation;

namespace PetitesVictoires.Api.Users.Get;

public sealed class GetUserByIdValidator : Validator<GetUserByIdRequest>
{
    public GetUserByIdValidator()
    {
        RuleFor(r => r.UserId)
            .GreaterThan(0);
    }
}