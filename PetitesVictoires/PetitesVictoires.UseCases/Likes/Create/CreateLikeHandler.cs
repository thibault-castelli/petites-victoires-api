using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using Microsoft.Extensions.Caching.Distributed;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.LikeAggregate.Specifications;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.UseCases.Likes.Create;

public class CreateLikeHandler(
    IRepository<Like> likesRepository,
    IReadRepository<Post> postRepository,
    IDistributedCache cache)
    : ICommandHandler<CreateLikeCommand, Result<LikeId>>
{
    public async ValueTask<Result<LikeId>> Handle(CreateLikeCommand command, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(command.PostId, cancellationToken);
        if (post is null) return Result.NotFound("Post not found");

        var likeAlreadyExisting = await likesRepository.FirstOrDefaultAsync(
            new LikeByUserAndPostSpecification(command.UserId, command.PostId),
            cancellationToken
        );
        if (likeAlreadyExisting is not null) return Result.Conflict("This like already exists");

        var newLike = new Like(command.UserId, command.PostId);
        await likesRepository.AddAsync(newLike, cancellationToken);

        await cache.RemoveAsync($"{Constants.PostCachePrefix}{post.Id.Value}", cancellationToken);

        return newLike.Id;
    }
}
