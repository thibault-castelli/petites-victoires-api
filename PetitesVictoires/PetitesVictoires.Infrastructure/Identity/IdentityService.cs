using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Infrastructure.Identity;

internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IIdentityService
{
    public async Task<Result<UserId>> CreateUserAsync(Email emailAddress, UserName name, string password,
        CancellationToken cancellationToken)
    {
        var newUser = new ApplicationUser { UserName = name.Value, Email = emailAddress.Value };
        var result = await userManager.CreateAsync(newUser, password);

        return result.Succeeded
            ? Result.Success(UserId.From(newUser.Id))
            : Result.Invalid(result.Errors.Select(e => new ValidationError(e.Description)).ToList());
    }

    public async Task<Result<AuthenticatedUser>> ValidateCredentialsAsync(Email emailAddress, string password,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(emailAddress.Value);
        if (user is null) return Result.Unauthorized();

        var result = await signInManager.CheckPasswordSignInAsync(user, password, true);

        if (result.Succeeded) return new AuthenticatedUser(user.Id, user.UserName!, user.Email!);
        return result.IsLockedOut ? Result.Error("Account locked. Try again later") : Result.Unauthorized();
    }

    public async Task<Result> ChangePasswordAsync(UserId userId, string currentPassword, string newPassword)
    {
        var userToUpdate = await userManager.FindByIdAsync(userId.Value.ToString());
        if (userToUpdate is null) return Result.NotFound();

        var result = await userManager.ChangePasswordAsync(userToUpdate, currentPassword!, newPassword!);
        return result.Succeeded
            ? Result.Success()
            : Result.Invalid(result.Errors.Select(e => new ValidationError(e.Description)).ToList());
    }

    public async Task<Result> DeleteUserAsync(UserId userId, CancellationToken cancellationToken)
    {
        var userToDelete = await userManager.FindByIdAsync(userId.Value.ToString());
        if (userToDelete is null) return Result.NotFound();

        var result = await userManager.DeleteAsync(userToDelete);
        return result.Succeeded
            ? Result.Success()
            : Result.Invalid(result.Errors.Select(e => new ValidationError(e.Description)).ToList());
    }

    public async Task<Result> UpdateUserAsync(UserId userId, Email emailAddress, UserName name
    )
    {
        var userToUpdate = await userManager.FindByIdAsync(userId.Value.ToString());
        if (userToUpdate is null) return Result.NotFound();

        userToUpdate.Email = emailAddress.Value;
        userToUpdate.UserName = name.Value;

        var result = await userManager.UpdateAsync(userToUpdate);
        return result.Succeeded
            ? Result.Success()
            : Result.Invalid(result.Errors.Select(e => new ValidationError(e.Description)).ToList());
    }
}
