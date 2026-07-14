using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Common;
using PetitesVictoires.UseCases.Users;
using PetitesVictoires.UseCases.Users.List;

namespace PetitesVictoires.Infrastructure.Queries;

public class ListUsersQueryService(PetitesVictoiresDbContext dbContext) : IListUsersQueryService
{
    public async Task<PagedResult<UserDto>> ListAsync(
        ListQueryParams listQueryParams,
        ListUsersCriteria listUsersCriteria,
        CancellationToken cancellationToken)
    {
        var page = listQueryParams.Page ?? 1;
        var countPerPage = listQueryParams.CountPerPage ?? Constants.DefaultPageSize;

        List<UserDto> users;
        int totalEntityCount;

        if (!string.IsNullOrWhiteSpace(listUsersCriteria.Search))
        {
            var trimmedSearch = listUsersCriteria.Search.Trim().Replace("%", "\\%").Replace("_", "\\_");
            users = await GetSearchedUsers(page, countPerPage, trimmedSearch, cancellationToken);
            totalEntityCount = await CountSearchedUsers(trimmedSearch, cancellationToken);
        }
        else
        {
            users = await GetUsersDefaultList(page, countPerPage, cancellationToken);
            totalEntityCount = await dbContext.Users.CountAsync(cancellationToken);
        }

        var totalPages = (int)Math.Ceiling(totalEntityCount / (double)countPerPage);
        var result = new PagedResult<UserDto>(users, page, countPerPage, totalEntityCount, totalPages);

        return result;
    }

    private async Task<List<UserDto>> GetSearchedUsers(
        int page,
        int countPerPage,
        string search,
        CancellationToken cancellationToken)
    {
        var prefix = $"{search}%";
        var anywhere = $"%{search}%";

        // Use raw SQL to prevent vogen conversion failure when comparing with search term
        // ILIKE -> 0 = exact
        // ILIKE -> 1 = starts-with
        // ILIKE -> 2 = contains
        var searchedUsers = await dbContext.Users
            .FromSql($"""
                      SELECT * FROM "Users"
                      WHERE ("EmailAddress" ILIKE {anywhere} OR "Name" ILIKE {anywhere})
                      ORDER BY
                        CASE
                          WHEN "EmailAddress" ILIKE {search}   OR "Name" ILIKE {search}   THEN 0
                          WHEN "EmailAddress" ILIKE {prefix} OR "Name" ILIKE {prefix} THEN 1
                          ELSE 2
                        END,
                        "Id"
                      LIMIT {countPerPage} OFFSET {(page - 1) * countPerPage}
                      """)
            .IgnoreQueryFilters()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var items = searchedUsers
            .Select(u => new UserDto(u.Id, u.EmailAddress, u.Name, u.CreatedAt))
            .ToList();

        return items;
    }

    private Task<int> CountSearchedUsers(string search, CancellationToken cancellationToken)
    {
        var anywhere = $"%{search}%";
        return dbContext.Users
            .FromSql($"""SELECT * FROM "Users" WHERE "EmailAddress" ILIKE {anywhere} OR "Name" ILIKE {anywhere}""")
            .CountAsync(cancellationToken);
    }

    private async Task<List<UserDto>> GetUsersDefaultList(
        int page,
        int countPerPage,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .OrderBy(u => u.Id)
            .Skip((page - 1) * countPerPage)
            .Take(countPerPage)
            .Select(u => new UserDto(u.Id, u.EmailAddress, u.Name, u.CreatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
