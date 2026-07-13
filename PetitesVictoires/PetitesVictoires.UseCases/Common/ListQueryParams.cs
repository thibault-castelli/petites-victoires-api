namespace PetitesVictoires.UseCases.Common;

public record ListQueryParams(int? Page = 1, int? CountPerPage = Constants.DefaultPageSize);
