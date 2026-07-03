using FastEndpoints;
using FluentValidation;

namespace PetitesVictoires.Api.Likes.Delete;

public sealed class DeleteLikeValidator : Validator<DeleteLikeRequest>
{
    public DeleteLikeValidator()
    {
        RuleFor(r => r.PostId)
            .GreaterThan(0);
    }
}
