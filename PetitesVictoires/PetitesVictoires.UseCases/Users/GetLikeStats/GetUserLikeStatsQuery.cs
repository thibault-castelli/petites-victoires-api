using Ardalis.Result;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Common;

namespace PetitesVictoires.UseCases.Users.GetLikeStats;

public record GetUserLikeStatsQuery(UserId UserId) : ICachedQuery<Result<UserLikeStatsDto>>
{
    public string CacheKey => $"{Constants.UserLikeStatsCachePrefix}{UserId}";
    public TimeSpan? CacheTimeout => TimeSpan.FromMinutes(5);
}
