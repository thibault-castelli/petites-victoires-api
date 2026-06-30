using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Identity;

internal sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<Result<UserId>> CreateUserAsync(Email email, UserName name, string password,
        CancellationToken cancellationToken)
    {
        var user = new ApplicationUser { UserName = name.Value, Email = email.Value };
        var result = await userManager.CreateAsync(user, password);

        return result.Succeeded
            ? Result.Success(UserId.From(user.Id))
            : Result.Invalid(result.Errors.Select(e => new ValidationError(e.Description)).ToList());
    }
}
