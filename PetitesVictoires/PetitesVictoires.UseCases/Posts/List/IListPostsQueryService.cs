using PetitesVictoires.UseCases.Common;

namespace PetitesVictoires.UseCases.Posts.List;

public interface IListPostsQueryService
{
    Task<PagedResult<PostDto>> ListAsync(
        ListQueryParams listQueryParams,
        ListPostsCriteria criteria,
        CancellationToken cancellationToken
    );
}