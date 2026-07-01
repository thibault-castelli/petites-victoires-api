namespace PetitesVictoires.UseCases;

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int CountPerPage,
    int TotalEntityCount,
    int TotalPages);