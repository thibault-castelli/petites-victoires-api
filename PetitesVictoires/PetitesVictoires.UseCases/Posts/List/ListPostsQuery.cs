using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Common;

namespace PetitesVictoires.UseCases.Posts.List;

public record ListPostsQuery(ListQueryParams ListQueryParams, ListPostsCriteria ListPostsCriteria)
    : IQuery<Result<PagedResult<PostDto>>>;

public record ListPostsCriteria(PostSortBy SortBy, UserId? LikedBy = null, UserId? CreatedBy = null);
