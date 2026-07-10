using FastEndpoints;
using FluentValidation;

namespace PetitesVictoires.Api.Users.GetLikeStats;

public sealed class GetUserLikeStatsValidator : Validator<GetUserLikeStatsRequest>
{
    public GetUserLikeStatsValidator()
    {
        RuleFor(request => request.UserId)
            .GreaterThan(0);
    }
}
