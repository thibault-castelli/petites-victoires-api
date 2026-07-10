using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.UseCases.Users;
using PetitesVictoires.UseCases.Users.GetLikeStats;

namespace PetitesVictoires.Infrastructure.Queries;

public class GetUserLikeStatsQueryService(PetitesVictoiresDbContext dbContext) : IGetUserLikeStatsQueryService
{
    public async Task<UserLikeStatsDto> GetUserLikeStatsAsync(UserId userId)
    {
        var likesGiven = await dbContext.Likes.CountAsync(l => l.UserId == userId);

        var likesReceived = await dbContext.Posts
            .Where(p => p.UserId == userId)
            .SumAsync(p => dbContext.Likes.Count(l => l.PostId == p.Id));

        return new UserLikeStatsDto(likesGiven, likesReceived);
    }
}
