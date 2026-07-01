using FastEndpoints;
using FluentValidation;

namespace PetitesVictoires.Api.Users.GetUserById;

public class GetUserByIdValidator : Validator<GetUserByIdRequest>
{
    public GetUserByIdValidator()
    {
        RuleFor(r => r.UserId)
            .GreaterThan(0);
    }
}
