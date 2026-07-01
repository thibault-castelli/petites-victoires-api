using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.Common;

namespace PetitesVictoires.UseCases.Users.SignIn;

public record SignInCommand(Email EmailAddress, string Password) : ICommand<Result<AuthenticatedUser>>;
