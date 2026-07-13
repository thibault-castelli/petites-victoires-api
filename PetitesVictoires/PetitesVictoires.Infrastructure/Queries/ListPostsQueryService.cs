using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Common;
using PetitesVictoires.UseCases.Posts;
using PetitesVictoires.UseCases.Posts.List;

namespace PetitesVictoires.Infrastructure.Queries;

public class ListPostsQueryService(PetitesVictoiresDbContext dbContext) : IListPostsQueryService
{
    public async Task<PagedResult<PostDto>> ListAsync(
        ListQueryParams listQueryParams,
        ListPostsCriteria criteria,
        CancellationToken cancellationToken)
    {
        var page = listQueryParams.Page ?? 1;
        var countPerPage = listQueryParams.CountPerPage ?? Constants.DefaultPageSize;
        var filteredPosts = GetFilteredPosts(criteria);
        var orderedPosts = GetOrderedPosts(filteredPosts, criteria);

        var items = await orderedPosts
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
            .ToListAsync(cancellationToken);

        var totalEntityCount = await filteredPosts.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalEntityCount / (double)countPerPage);
        var result = new PagedResult<PostDto>(items, page, countPerPage,
            totalEntityCount, totalPages);

        return result;
    }

    private IQueryable<Post> GetFilteredPosts(ListPostsCriteria criteria)
    {
        IQueryable<Post> listedPosts = dbContext.Posts;

        if (criteria.CreatedBy is { } createdBy)
            listedPosts = listedPosts.Where(p => p.UserId == createdBy);

        if (criteria.LikedBy is { } likedBy)
            listedPosts = listedPosts.Where(p => dbContext.Likes.Any(l => l.PostId == p.Id && l.UserId == likedBy));

        return listedPosts;
    }

    private IOrderedQueryable<Post> GetOrderedPosts(IQueryable<Post> filteredPosts, ListPostsCriteria criteria)
    {
        return criteria.SortBy switch
        {
            PostSortBy.LikesCount => filteredPosts.OrderByDescending(p => dbContext.Likes.Count(l => l.PostId == p.Id))
                .ThenByDescending(p => p.Id),
            PostSortBy.CreatedAt => filteredPosts.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id),
            _ => filteredPosts.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id)
        };
    }
}
