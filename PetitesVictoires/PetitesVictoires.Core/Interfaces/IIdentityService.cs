using Ardalis.Result;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Core.Interfaces;

public interface IIdentityService
{
    Task<Result<UserId>> CreateUserAsync(Email emailAddress, UserName name, string password,
        CancellationToken cancellationToken);

    Task<Result<AuthenticatedUser>> ValidateCredentialsAsync(Email emailAddress, string password,
        CancellationToken cancellationToken);

    Task<Result> UpdateUserAsync(UserId userId, Email emailAddress, UserName name);

    Task<Result> ChangePasswordAsync(UserId userId, string currentPassword, string newPassword);

    Task<Result> DeleteUserAsync(UserId userId, CancellationToken cancellationToken);
}