using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Create;

public class CreateUserHandler(IRepository<User> repository, IIdentityService identityService, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateUserCommand, Result<UserId>>
{
    public async ValueTask<Result<UserId>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var identityResult =
            await identityService.CreateUserAsync(request.Email, request.Name, request.Password, cancellationToken);
        if (!identityResult.IsSuccess) return identityResult;

        var newUser = new User(identityResult.Value, request.Email, request.Name);
        var createdUser = await repository.AddAsync(newUser, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return createdUser.Id;
    }
}
