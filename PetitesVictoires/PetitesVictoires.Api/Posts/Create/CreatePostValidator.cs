using FastEndpoints;
using FluentValidation;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.Api.Posts.Create;

public sealed class CreatePostValidator : Validator<CreatePostRequest>
{
    public CreatePostValidator()
    {
        RuleFor(r => r.Content)
            .NotEmpty()
            .WithMessage("Content is required")
            .MaximumLength(PostContent.MaxLength);
    }
}
