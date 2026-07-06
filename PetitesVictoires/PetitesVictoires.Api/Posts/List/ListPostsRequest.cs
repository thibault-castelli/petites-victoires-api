using FastEndpoints;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Posts.List;

public record ListPostsRequest
{
    public const string Route = "/Posts";

    // Bind to ?page=
    [BindFrom("page")] public int Page { get; init; } = 1;

    // Bind to ?count_per_page=
    [BindFrom("count_per_page")] public int CountPerPage { get; init; } = Constants.DefaultPageSize;
}
