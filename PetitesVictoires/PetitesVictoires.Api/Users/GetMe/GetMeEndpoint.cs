using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Get;

namespace PetitesVictoires.Api.Users.GetMe;

public class GetMeEndpoint(IMediator mediator)
    : EndpointWithoutRequest<Results<Ok<UserRecord>, NotFound, ProblemHttpResult>, GetMeMapper>
{
    public const string Route = "Users/Me";

    public override void Configure()
    {
        Get(Route);
        Summary(s =>
        {
            s.Summary = "Gets the currently signed-in user";
            s.ResponseExamples[200] = new UserRecord(1, "example@mail.com", "example", DateTime.Now);
            s.Responses[200] = "Current user found and returned successfully.";
            s.Responses[401] = "No authenticated user.";
            s.Responses[404] = "The authenticated user no longer exists.";
        });
        Tags("Users");
        Description(b => b
            .WithName("GetMe")
            .Produces<UserRecord>(200, "application/json")
            .ProducesProblem(401)
            .ProducesProblem(404)
        );
    }

    public override async Task<Results<Ok<UserRecord>, NotFound, ProblemHttpResult>> ExecuteAsync(
        CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(UserId.From(User.GetAuthenticatedUserId()));
        var result = await mediator.Send(query, cancellationToken);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}
