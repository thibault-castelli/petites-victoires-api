using FastEndpoints;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Common;
using PetitesVictoires.UseCases.Users;
using PetitesVictoires.UseCases.Users.List;

namespace PetitesVictoires.Api.Users.List;

public sealed class ListUsersMapper : Mapper<ListUsersRequest, ListUsersResponse, PagedResult<UserDto>>
{
    public override ListUsersResponse FromEntity(PagedResult<UserDto> pagedResultEntity)
    {
        var items = pagedResultEntity.Items
            .Select(u => new UserRecord(u.Id.Value, u.EmailAddress.Value, u.Name.Value, u.CreatedAt))
            .ToList();

        return new ListUsersResponse(
            items,
            pagedResultEntity.Page,
            pagedResultEntity.CountPerPage,
            pagedResultEntity.TotalEntityCount,
            pagedResultEntity.TotalPages
        );
    }

    public ListUsersQuery ToQuery(ListUsersRequest request)
    {
        return new ListUsersQuery(
            new ListQueryParams(request.Page, request.CountPerPage),
            new ListUsersCriteria(request.Search)
        );
    }
}
