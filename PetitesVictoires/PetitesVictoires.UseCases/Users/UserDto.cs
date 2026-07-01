using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users;

public record UserDto(UserId Id, Email EmailAddress, UserName Name, DateTime CreatedAt);
