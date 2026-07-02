using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Update;

public class UpdateUserHandler(IRepository<User> repository) : ICommandHandler<UpdateUserCommand, Result<UserDto>>
{
    public async ValueTask<Result<UserDto>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await repository.GetByIdAsync(command.UserId, cancellationToken);
        if (existingUser is null) return Result.NotFound();

        existingUser.UpdateEmailAddress(command.EmailAddress);
        existingUser.UpdateName(command.Name);
        existingUser.MarkUpdated();

        await repository.UpdateAsync(existingUser, cancellationToken);

        return new UserDto(existingUser.Id, existingUser.EmailAddress, existingUser.Name, existingUser.CreatedAt);
    }
}
