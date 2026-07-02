using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Delete;

public class DeleteUserHandler(IRepository<User> repository) : ICommandHandler<DeleteUserCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var userToDelete = await repository.GetByIdAsync(command.UserId, cancellationToken);
        if (userToDelete is null) return Result.NotFound();

        await repository.DeleteAsync(userToDelete, cancellationToken);
        return Result.Success();
    }
}
