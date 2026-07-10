using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using Microsoft.Extensions.Caching.Distributed;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Delete;

public class DeleteUserHandler(
    IRepository<User> repository,
    IIdentityService identityService,
    IUnitOfWork unitOfWork,
    IDistributedCache cache)
    : ICommandHandler<DeleteUserCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var userToDelete = await repository.GetByIdAsync(command.UserId, cancellationToken);
        if (userToDelete is null) return Result.NotFound();

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var identityResult = await identityService.DeleteUserAsync(userToDelete.Id, cancellationToken);
        if (!identityResult.IsSuccess) return identityResult;

        await repository.DeleteAsync(userToDelete, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        await cache.RemoveAsync($"{Constants.UserCachePrefix}{userToDelete.Id}", cancellationToken);

        return Result.Success();
    }
}
