using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Update;

public class UpdateUserHandler(IRepository<User> repository, IIdentityService identityService, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateUserCommand, Result<UserDto>>
{
    public async ValueTask<Result<UserDto>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await repository.GetByIdAsync(command.UserId, cancellationToken);
        if (existingUser is null) return Result.NotFound("User not found");

        existingUser.UpdateEmailAddress(command.EmailAddress);
        existingUser.UpdateName(command.Name);
        existingUser.MarkUpdated();

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var identityUpdateResult =
            await identityService.UpdateUserAsync(command.UserId, command.EmailAddress, command.Name);
        if (!identityUpdateResult.IsSuccess) return identityUpdateResult;

        var shouldChangePassword = command.CurrentPassword is not null && command.NewPassword is not null &&
                                   command.CurrentPassword.Trim() != command.NewPassword.Trim();
        if (shouldChangePassword)
        {
            var identityPasswordResult = await identityService.ChangePasswordAsync(command.UserId,
                command.CurrentPassword!.Trim(), command.NewPassword!.Trim());
            if (!identityPasswordResult.IsSuccess) return identityPasswordResult;
        }

        await repository.UpdateAsync(existingUser, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return new UserDto(existingUser.Id, existingUser.EmailAddress, existingUser.Name, existingUser.CreatedAt);
    }
}
