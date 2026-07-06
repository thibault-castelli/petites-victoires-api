using FastEndpoints;
using FluentValidation;

namespace PetitesVictoires.Api.Posts.Delete;

public class DeletePostValidator : Validator<DeletePostRequest>
{
    public DeletePostValidator()
    {
        RuleFor(x => x.PostId)
            .GreaterThan(0);
    }
}
