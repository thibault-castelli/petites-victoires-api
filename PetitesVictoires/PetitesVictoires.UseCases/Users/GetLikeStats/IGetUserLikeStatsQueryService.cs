using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Users.GetLikeStats;

public interface IGetUserLikeStatsQueryService
{
    Task<UserLikeStatsDto> GetUserLikeStatsAsync(UserId userId, CancellationToken cancellationToken);
}
