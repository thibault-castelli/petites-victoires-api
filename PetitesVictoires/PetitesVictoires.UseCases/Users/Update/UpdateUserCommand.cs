using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Update;

public record UpdateUserCommand(UserId UserId, Email EmailAddress, UserName Name) : ICommand<Result<UserDto>>;
