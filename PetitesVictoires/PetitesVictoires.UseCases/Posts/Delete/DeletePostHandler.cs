using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.PostAggregate.Specifications;

namespace PetitesVictoires.UseCases.Posts.Delete;

public class DeletePostHandler(IRepository<Post> repository)
    : ICommandHandler<DeletePostCommand, Result>
{
    public async ValueTask<Result> Handle(DeletePostCommand command, CancellationToken cancellationToken)
    {
        var postToSoftDelete = await repository.FirstOrDefaultAsync(
            new PostByIdAndUserSpecification(command.PostId, command.UserId),
            cancellationToken
        );
        if (postToSoftDelete is null)
        {
            var postOwnedByAnotherUser = await repository.GetByIdAsync(command.PostId, cancellationToken);
            return postOwnedByAnotherUser is null
                ? Result.NotFound("Post not found")
                : Result.Forbidden("You cannot update this post");
        }

        postToSoftDelete.MarkSoftDeleted();

        await repository.UpdateAsync(postToSoftDelete, cancellationToken);

        return Result.Success();
    }
}
