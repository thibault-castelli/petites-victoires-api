using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.GetLikeStats;

namespace PetitesVictoires.Api.Users.GetLikeStats;

public class GetUserLikeStatsEndpoint(IMediator mediator)
    : Endpoint<GetUserLikeStatsRequest, Results<Ok<UserLikeStatsRecord>, NotFound, ProblemHttpResult>,
        GetUserLikeStatsMapper>
{
    public override void Configure()
    {
        Get(GetUserLikeStatsRequest.Route);
        Summary(s =>
        {
            s.Summary = "Gets a user like stats (total likes given and total likes received)";
            s.ExampleRequest =
                new GetUserLikeStatsRequest { UserId = 1 };
            s.ResponseExamples[200] = new UserRecord(1, "example@mail.com", "example", DateTime.Now);
            s.Responses[200] = "User like stats found and returned successfully.";
            s.Responses[401] = "Unauthorized, user not signed in.";
        });
        Tags("Users");
        Description(b => b
            .Accepts<GetUserLikeStatsRequest>()
            .Produces<UserRecord>(200, "application/json")
            .ProducesProblem(401)
        );
    }

    public override async Task<Results<Ok<UserLikeStatsRecord>, NotFound, ProblemHttpResult>> ExecuteAsync(
        GetUserLikeStatsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetUserLikeStatsQuery(UserId.From(request.UserId));
        var result = await mediator.Send(query, cancellationToken);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}
