using Ardalis.Result;
using Mediator;

namespace PetitesVictoires.UseCases.Users.List;

public record ListUsersQuery(int? Page = 1, int? CountPerPage = Constants.DefaultPageSize)
    : IQuery<Result<PagedResult<UserDto>>>;
