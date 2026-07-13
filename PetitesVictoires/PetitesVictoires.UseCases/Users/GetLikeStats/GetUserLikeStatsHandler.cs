using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Core.UserAggregate.Specifications;

namespace PetitesVictoires.UseCases.Users.GetLikeStats;

public class GetUserLikeStatsHandler(IReadRepository<User> repository, IGetUserLikeStatsQueryService queryService)
    : IQueryHandler<GetUserLikeStatsQuery, Result<UserLikeStatsDto>>
{
    public async ValueTask<Result<UserLikeStatsDto>> Handle(GetUserLikeStatsQuery query,
        CancellationToken cancellationToken)
    {
        var isUserFound = await repository.AnyAsync(new UserByIdSpecification(query.UserId), cancellationToken);
        if (!isUserFound) return Result.NotFound("User not found");

        var likeStats = await queryService.GetUserLikeStatsAsync(query.UserId, cancellationToken);

        return Result.Success(likeStats);
    }
}
