using FastEndpoints;
using FluentValidation;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.Api.Posts.Update;

public sealed class UpdatePostValidator : Validator<UpdatePostRequest>
{
    public UpdatePostValidator()
    {
        RuleFor(r => r.PostId)
            .GreaterThan(0);

        RuleFor(r => r.PostContent)
            .NotEmpty()
            .WithMessage("Content is required")
            .MaximumLength(PostContent.MaxLength);
    }
}
