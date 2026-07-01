using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.Get;

public record GetUserQuery(UserId UserId) : IQuery<Result<UserDto>>;