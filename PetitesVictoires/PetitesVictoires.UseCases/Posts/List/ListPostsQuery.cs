using Ardalis.Result;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Behaviors;
using PetitesVictoires.UseCases.Common;

namespace PetitesVictoires.UseCases.Posts.List;

public record ListPostsQuery(ListQueryParams ListQueryParams, ListPostsCriteria ListPostsCriteria)
    : ICachedQuery<Result<PagedResult<PostDto>>>
{
    public string CacheKey
    {
        get
        {
            var page = ListQueryParams.Page ?? 1;
            var countPerPage = ListQueryParams.CountPerPage ?? Constants.DefaultPageSize;

            var isCommonCase =
                ListPostsCriteria.LikedBy is null &&
                ListPostsCriteria.CreatedBy is null &&
                countPerPage == Constants.DefaultPageSize &&
                page <= Constants.MaxCachedPage;

            return isCommonCase ? $"posts:list:sort{ListPostsCriteria.SortBy}:page:{page}" : string.Empty;
        }
    }

    public TimeSpan? CacheTimeout => TimeSpan.FromSeconds(15);
}

public record ListPostsCriteria(PostSortBy SortBy, UserId? LikedBy = null, UserId? CreatedBy = null);
