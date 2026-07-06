using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Posts.List;

public record ListPostsResponse(
    IReadOnlyList<PostRecord> Items,
    int Page,
    int CountPerPage,
    int TotalEntityCount,
    int TotalPages) : PagedResult<PostRecord>(Items, Page, CountPerPage, TotalEntityCount, TotalPages);
