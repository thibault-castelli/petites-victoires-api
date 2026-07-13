using Ardalis.Result;
using PetitesVictoires.UseCases.Behaviors;
using PetitesVictoires.UseCases.Common;

namespace PetitesVictoires.UseCases.Users.List;

public record ListUsersQuery(ListQueryParams ListQueryParams, ListUsersCriteria ListUsersCriteria)
    : ICachedQuery<Result<PagedResult<UserDto>>>
{
    public string CacheKey
    {
        get
        {
            var page = ListQueryParams.Page ?? 1;
            var countPerPage = ListQueryParams.CountPerPage ?? Constants.DefaultPageSize;

            var isCommonCase =
                string.IsNullOrWhiteSpace(ListUsersCriteria.Search) &&
                countPerPage == Constants.DefaultPageSize &&
                page <= Constants.MaxCachedPage;

            return isCommonCase ? $"users:list:p{page}" : string.Empty;
        }
    }

    public TimeSpan? CacheTimeout => TimeSpan.FromSeconds(15);
}

public record ListUsersCriteria(string? Search);
