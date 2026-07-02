using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Create;

public class CreateUserHandler(IRepository<User> repository, IIdentityService identityService, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateUserCommand, Result<UserId>>
{
    public async ValueTask<Result<UserId>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var identityResult =
            await identityService.CreateUserAsync(command.EmailAddress, command.Name, command.Password.Trim(),
                cancellationToken);
        if (!identityResult.IsSuccess) return identityResult;

        var newUser = new User(identityResult.Value, command.EmailAddress, command.Name);
        var createdUser = await repository.AddAsync(newUser, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return createdUser.Id;
    }
}
