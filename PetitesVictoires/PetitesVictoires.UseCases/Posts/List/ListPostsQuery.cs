using Ardalis.Result;
using Mediator;

namespace PetitesVictoires.UseCases.Posts.List;

public record ListPostsQuery(int? Page = 1, int? CountPerPage = Constants.DefaultPageSize)
    : IQuery<Result<PagedResult<PostDto>>>;
