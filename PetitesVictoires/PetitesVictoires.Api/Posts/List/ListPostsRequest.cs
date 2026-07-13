using FastEndpoints;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Posts.List;

public record ListPostsRequest
{
    public const string Route = "/Posts";

    [BindFrom("page")] public int Page { get; init; } = 1;

    [BindFrom("count_per_page")] public int CountPerPage { get; init; } = Constants.DefaultPageSize;

    [BindFrom("sort_by")] public string? SortBy { get; init; }

    [BindFrom("liked_by")] public int? LikedBy { get; init; }

    [BindFrom("created_by")] public int? CreatedBy { get; init; }
}
