using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.PostAggregate.Specifications;

namespace PetitesVictoires.UseCases.Posts.Get;

public class GetPostHandler(IReadRepository<Post> repository) : IQueryHandler<GetPostQuery, Result<PostDto>>
{
    public async ValueTask<Result<PostDto>> Handle(GetPostQuery query, CancellationToken ct)
    {
        var specification = new PostByIdSpecification(query.PostId);
        var entity = await repository.FirstOrDefaultAsync(specification, ct);
        if (entity == null) return Result.NotFound();

        return new PostDto(entity.Id, entity.Content, entity.CreatedAt);
    }
}
