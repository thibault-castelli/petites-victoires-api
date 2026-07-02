using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Delete;

public record DeleteUserCommand(UserId UserId) : ICommand<Result>;
