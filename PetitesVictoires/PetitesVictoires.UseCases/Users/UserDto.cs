using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users;

public record UserDto(UserId Id, Email emailAddress, UserName UserName, DateTime CreatedAt);
