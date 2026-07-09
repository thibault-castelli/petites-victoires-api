using FastEndpoints;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts;

namespace PetitesVictoires.Api.Posts.List;

public sealed class ListPostsMapper : Mapper<ListPostsRequest, ListPostsResponse, PagedResult<PostDto>>
{
    public override ListPostsResponse FromEntity(PagedResult<PostDto> pagedResultEntity)
    {
        var items = pagedResultEntity.Items
            .Select(p => new PostRecord(
                p.Id.Value,
                p.Content.Value,
                p.UserId.Value,
                p.UserEmailAddress.Value,
                p.UserName.Value,
                p.LikesCount,
                p.CreatedAt)
            )
            .ToList();

        return new ListPostsResponse(
            items,
            pagedResultEntity.Page,
            pagedResultEntity.CountPerPage,
            pagedResultEntity.TotalEntityCount,
            pagedResultEntity.TotalPages
        );
    }
}
