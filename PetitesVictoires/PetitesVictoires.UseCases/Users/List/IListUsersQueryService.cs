using PetitesVictoires.UseCases.Common;

namespace PetitesVictoires.UseCases.Users.List;

public interface IListUsersQueryService
{
    Task<PagedResult<UserDto>> ListAsync(
        ListQueryParams listQueryParams,
        ListUsersCriteria listUsersCriteria,
        CancellationToken cancellationToken
    );
}
