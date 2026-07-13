using Ardalis.Result;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Behaviors;

namespace PetitesVictoires.UseCases.Users.Get;

public record GetUserQuery(UserId UserId) : ICachedQuery<Result<UserDto>>
{
    public string CacheKey => $"{Constants.UserCachePrefix}{UserId}";
    public TimeSpan? CacheTimeout => TimeSpan.FromMinutes(5);
}