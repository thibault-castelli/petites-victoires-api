using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Users.List;

public record ListUsersResponse(
    IReadOnlyList<UserRecord> Items,
    int Page,
    int CountPerPage,
    int TotalEntityCount,
    int TotalPages) : PagedResult<UserRecord>(Items, Page, CountPerPage, TotalEntityCount, TotalPages);