using FastEndpoints;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Users.List;

public record ListUsersRequest
{
    public const string Route = "/Users";

    [BindFrom("page")] public int Page { get; init; } = 1;

    [BindFrom("count_per_page")] public int CountPerPage { get; init; } = Constants.DefaultPageSize;

    [BindFrom("search")] public string? Search { get; init; }
}
