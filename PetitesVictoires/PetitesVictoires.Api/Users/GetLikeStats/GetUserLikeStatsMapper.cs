using FastEndpoints;
using PetitesVictoires.UseCases.Users;

namespace PetitesVictoires.Api.Users.GetLikeStats;

public sealed class GetUserLikeStatsMapper : Mapper<GetUserLikeStatsRequest, UserLikeStatsRecord, UserLikeStatsDto>
{
    public override UserLikeStatsRecord FromEntity(UserLikeStatsDto entity)
    {
        return new UserLikeStatsRecord(
            entity.LikesGiven,
            entity.LikesReceived
        );
    }
}
