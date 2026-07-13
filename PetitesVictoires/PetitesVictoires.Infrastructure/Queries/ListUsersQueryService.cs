using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Users;
using PetitesVictoires.UseCases.Users.List;

namespace PetitesVictoires.Infrastructure.Queries;

public class ListUsersQueryService(PetitesVictoiresDbContext dbContext) : IListUsersQueryService
{
    public async Task<PagedResult<UserDto>> ListAsync(int page, int countPerPage, CancellationToken cancellationToken)
    {
        var items = await dbContext.Users
            .OrderBy(u => u.Id)
            .Skip((page - 1) * countPerPage)
            .Take(countPerPage)
            .Select(u => new UserDto(u.Id, u.EmailAddress, u.Name, u.CreatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalEntityCount = await dbContext.Users.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalEntityCount / (double)countPerPage);
        var result = new PagedResult<UserDto>(items, page, countPerPage, totalEntityCount, totalPages);

        return result;
    }
}
