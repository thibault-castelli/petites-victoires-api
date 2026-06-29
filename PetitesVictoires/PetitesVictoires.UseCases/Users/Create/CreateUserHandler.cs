using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Create;

public class CreateUserHandler(IRepository<User> repository, IIdentityService identityService)
    : ICommandHandler<CreateUserCommand, Result<UserId>>
{
    public async ValueTask<Result<UserId>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var identityResult = await identityService.CreateUserAsync(request.Email, request.Password, cancellationToken);
        if (!identityResult.IsSuccess) return identityResult;

        var newUser = new User(identityResult.Value, request.Email, request.Name);
        var createdUser = await repository.AddAsync(newUser, cancellationToken);

        return createdUser.Id;
    }
}
