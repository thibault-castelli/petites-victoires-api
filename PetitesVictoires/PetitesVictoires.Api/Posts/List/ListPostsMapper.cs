using FastEndpoints;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Common;
using PetitesVictoires.UseCases.Posts;
using PetitesVictoires.UseCases.Posts.List;

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

    public ListPostsQuery ToQuery(ListPostsRequest request)
    {
        UserId? likedBy = request.LikedBy is { } lb ? UserId.From(lb) : null;
        UserId? createdBy = request.CreatedBy is { } cb ? UserId.From(cb) : null;

        return new ListPostsQuery(
            new ListQueryParams(request.Page, request.CountPerPage),
            new ListPostsCriteria(MapSortBy(request.SortBy), likedBy, createdBy)
        );
    }

    private PostSortBy MapSortBy(string? value)
    {
        return value switch
        {
            "created_at" => PostSortBy.CreatedAt,
            "likes_count" => PostSortBy.LikesCount,
            _ => PostSortBy.CreatedAt
        };
    }
}
