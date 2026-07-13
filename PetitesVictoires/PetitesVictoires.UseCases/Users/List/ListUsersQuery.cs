using Ardalis.Result;
using Mediator;
using PetitesVictoires.UseCases.Common;

namespace PetitesVictoires.UseCases.Users.List;

public record ListUsersQuery(ListQueryParams ListQueryParams, ListUsersCriteria ListUsersCriteria)
    : IQuery<Result<PagedResult<UserDto>>>;

public record ListUsersCriteria(string? Search);
