using Ardalis.Result;
using Mediator;

namespace PetitesVictoires.UseCases.Posts.List;

public class ListPostsHandler(IListPostsQueryService queryService)
    : IQueryHandler<ListPostsQuery, Result<PagedResult<PostDto>>>
{
    public async ValueTask<Result<PagedResult<PostDto>>> Handle(ListPostsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await queryService.ListAsync(query.Page ?? 1, query.CountPerPage ?? Constants.DefaultPageSize);

        return result;
    }
}
