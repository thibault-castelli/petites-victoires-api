using Ardalis.Result;
using Mediator;

namespace PetitesVictoires.UseCases.Users.List;

public class ListUsersHandler(IListUsersQueryService queryService)
    : IQueryHandler<ListUsersQuery, Result<PagedResult<UserDto>>>
{
    public async ValueTask<Result<PagedResult<UserDto>>> Handle(ListUsersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await queryService.ListAsync(
            query.ListQueryParams,
            query.ListUsersCriteria,
            cancellationToken
        );

        return result;
    }
}
