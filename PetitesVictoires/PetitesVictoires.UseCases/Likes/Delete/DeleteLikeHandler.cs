using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.LikeAggregate.Specifications;

namespace PetitesVictoires.UseCases.Likes.Delete;

public class DeleteLikeHandler(IRepository<Like> repository) : ICommandHandler<DeleteLikeCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteLikeCommand command, CancellationToken cancellationToken)
    {
        var likeToDelete = await repository.FirstOrDefaultAsync(
            new LikeByUserAndPostSpecification(command.UserId, command.PostId),
            cancellationToken
        );
        if (likeToDelete is null) return Result.NotFound("Like not found");

        await repository.DeleteAsync(likeToDelete, cancellationToken);

        return Result.Success();
    }
}
