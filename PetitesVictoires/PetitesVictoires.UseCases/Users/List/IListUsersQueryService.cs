namespace PetitesVictoires.UseCases.Users.List;

public interface IListUsersQueryService
{
    Task<PagedResult<UserDto>> ListAsync(int page, int countPerPage, CancellationToken cancellationToken);
}
