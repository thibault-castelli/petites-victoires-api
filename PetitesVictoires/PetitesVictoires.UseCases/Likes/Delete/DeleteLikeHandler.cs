using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using Microsoft.Extensions.Caching.Distributed;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.LikeAggregate.Specifications;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.UseCases.Likes.Delete;

public class DeleteLikeHandler(
    IRepository<Like> likeRepository,
    IReadRepository<Post> postRepository,
    IDistributedCache cache)
    : ICommandHandler<DeleteLikeCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteLikeCommand command, CancellationToken cancellationToken)
    {
        var likeToDelete = await likeRepository.FirstOrDefaultAsync(
            new LikeByUserAndPostSpecification(command.UserId, command.PostId),
            cancellationToken
        );
        if (likeToDelete is null) return Result.NotFound("Like not found");

        var linkedPost = await postRepository.GetByIdAsync(likeToDelete.PostId, cancellationToken);

        await likeRepository.DeleteAsync(likeToDelete, cancellationToken);

        await cache.RemoveAsync($"{Constants.PostCachePrefix}{command.PostId.Value}", cancellationToken);
        await cache.RemoveAsync($"{Constants.UserLikeStatsCachePrefix}{command.UserId}", cancellationToken);
        await cache.RemoveAsync($"{Constants.UserLikeStatsCachePrefix}{linkedPost!.UserId}", cancellationToken);

        return Result.Success();
    }
}
