namespace PetitesVictoires.UseCases.Posts.List;

public interface IListPostsQueryService
{
    Task<PagedResult<PostDto>> ListAsync(int page, int countPerPage);
}
