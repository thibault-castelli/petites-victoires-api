using FastEndpoints;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Users.List;

public record ListUsersRequest
{
    public const string Route = "/Users";

    // Bind to ?page=
    [BindFrom("page")] public int Page { get; init; } = 1;

    // Bind to ?count_per_page=
    [BindFrom("count_per_page")] public int CountPerPage { get; init; } = Constants.DefaultPageSize;
}