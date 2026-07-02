using FastEndpoints;
using FluentValidation;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Users.ListUsers;

public sealed class ListUsersValidator : Validator<ListUsersRequest>
{
    public ListUsersValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than 1");

        RuleFor(x => x.CountPerPage)
            .InclusiveBetween(1, Constants.MaxPageSize)
            .WithMessage($"Count per page must be between 1 and {Constants.MaxPageSize}");
    }
}
