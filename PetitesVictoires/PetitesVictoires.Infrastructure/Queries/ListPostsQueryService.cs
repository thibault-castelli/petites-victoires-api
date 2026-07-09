using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts;
using PetitesVictoires.UseCases.Posts.List;

namespace PetitesVictoires.Infrastructure.Queries;

public class ListPostsQueryService(PetitesVictoiresDbContext dbContext) : IListPostsQueryService
{
    public async Task<PagedResult<PostDto>> ListAsync(int page, int countPerPage)
    {
        var items = await dbContext.Posts
            .OrderBy(p => p.Id)
            .Skip((page - 1) * countPerPage)
            .Take(countPerPage)
            .Join(dbContext.Users,
                post => post.UserId,
                user => user.Id,
                (post, user) =>
                    new PostDto(
                        post.Id,
                        post.Content,
                        user.Id,
                        user.EmailAddress,
                        user.Name,
                        post.CreatedAt,
                        dbContext.Likes.Count(l => l.PostId == post.Id)
                    )
            )
            .AsNoTracking()
            .ToListAsync();

        var totalEntityCount = await dbContext.Posts.CountAsync();
        var totalPages = (int)Math.Ceiling(totalEntityCount / (double)countPerPage);
        var result = new PagedResult<PostDto>(items, page, countPerPage, totalEntityCount, totalPages);

        return result;
    }
}
