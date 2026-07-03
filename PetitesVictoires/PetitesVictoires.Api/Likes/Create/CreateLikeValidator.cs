using FastEndpoints;
using FluentValidation;

namespace PetitesVictoires.Api.Likes.Create;

public sealed class CreateLikeValidator : Validator<CreateLikeRequest>
{
    public CreateLikeValidator()
    {
        RuleFor(r => r.PostId)
            .GreaterThan(0);
    }
}
