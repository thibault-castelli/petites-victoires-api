using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Create;

public record CreateUserCommand(Email EmailAddress, UserName Name, string Password) : ICommand<Result<UserId>>;
