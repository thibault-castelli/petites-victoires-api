using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.UseCases.Posts;
using PetitesVictoires.UseCases.Posts.Get;

namespace PetitesVictoires.Infrastructure.Queries;

public class GetPostQueryService(PetitesVictoiresDbContext dbContext) : IGetPostQueryService
{
    public async Task<PostDto?> GetPostAsync(PostId postId)
    {
        return await dbContext.Posts
            .Where(p => p.Id == postId)
            .Join(dbContext.Users,
                post => post.UserId,
                user => user.Id,
                (post, user) =>
                    new PostDto(post.Id, post.Content, user.Id, user.EmailAddress, user.Name, post.CreatedAt))
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}
