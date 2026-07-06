using FastEndpoints;
using FluentValidation;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Posts.List;

public sealed class ListPostsValidator : Validator<ListPostsRequest>
{
    public ListPostsValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than 1");

        RuleFor(x => x.CountPerPage)
            .InclusiveBetween(1, Constants.MaxPageSize)
            .WithMessage($"Count per page must be between 1 and {Constants.MaxPageSize}");
    }
}
