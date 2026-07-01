using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;

namespace PetitesVictoires.UseCases.Users.SignIn;

public class SignInHandler(IIdentityService identityService) : ICommandHandler<SignInCommand, Result<AuthenticatedUser>>
{
    public async ValueTask<Result<AuthenticatedUser>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        return await identityService.ValidateCredentialsAsync(command.EmailAddress, command.Password,
            cancellationToken);
    }
}
