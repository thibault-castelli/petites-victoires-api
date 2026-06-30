using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.Common;

namespace PetitesVictoires.UseCases.Users.SignIn;

public record SignInCommand(Email Email, string Password) : ICommand<Result<AuthenticatedUser>>;