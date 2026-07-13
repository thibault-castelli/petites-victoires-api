using FastEndpoints;
using FluentValidation;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Posts.List;

public sealed class ListPostsValidator : Validator<ListPostsRequest>
{
    public static readonly string[] AllowedPostsSorts = ["created_at", "likes_count"];

    public ListPostsValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than 1");

        RuleFor(x => x.CountPerPage)
            .InclusiveBetween(1, Constants.MaxPageSize)
            .WithMessage($"Count per page must be between 1 and {Constants.MaxPageSize}");

        RuleFor(x => x.SortBy)
            .Must(v => v is null || AllowedPostsSorts.Contains(v))
            .WithMessage($"Sort by must be one of the following: {string.Join(", ", AllowedPostsSorts)}");

        RuleFor(x => x.LikedBy)
            .GreaterThan(0);

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0);
    }
}