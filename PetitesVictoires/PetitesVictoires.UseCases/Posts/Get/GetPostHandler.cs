using Ardalis.Result;
using Mediator;

namespace PetitesVictoires.UseCases.Posts.Get;

public class GetPostHandler(IGetPostQueryService queryService) : IQueryHandler<GetPostQuery, Result<PostDto>>
{
    public async ValueTask<Result<PostDto>> Handle(GetPostQuery query, CancellationToken cancellationToken)
    {
        var post = await queryService.GetPostAsync(query.PostId);
        return post is null ? Result.NotFound() : Result.Success(post);
    }
}
