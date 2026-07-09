using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using Microsoft.Extensions.Caching.Distributed;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.UseCases.Posts.Delete;

public class DeletePostHandler(IRepository<Post> repository, IDistributedCache cache)
    : ICommandHandler<DeletePostCommand, Result>
{
    public async ValueTask<Result> Handle(DeletePostCommand command, CancellationToken cancellationToken)
    {
        var postToSoftDelete = await repository.GetByIdAsync(command.PostId, cancellationToken);
        if (postToSoftDelete is null) return Result.NotFound();
        if (postToSoftDelete.UserId != command.UserId) return Result.Forbidden();

        postToSoftDelete.MarkSoftDeleted();

        await repository.UpdateAsync(postToSoftDelete, cancellationToken);
        await cache.RemoveAsync($"{Constants.PostCachePrefix}{postToSoftDelete.Id.Value}", cancellationToken);

        return Result.Success();
    }
}
