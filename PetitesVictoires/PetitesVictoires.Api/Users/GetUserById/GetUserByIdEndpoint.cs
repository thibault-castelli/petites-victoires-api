using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Get;

namespace PetitesVictoires.Api.Users.GetUserById;

public class GetUserByIdEndpoint(IMediator mediator)
    : Endpoint<GetUserByIdRequest, Results<Ok<UserRecord>, NotFound, ProblemHttpResult>, GetUserByIdMapper>
{
    public override void Configure()
    {
        Get(GetUserByIdRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Gets a User by Id";
            s.ExampleRequest =
                new GetUserByIdRequest { UserId = 1 };
            s.ResponseExamples[200] = new UserRecord(1, "example@mail.com", "example", DateTime.Now);
            s.Responses[200] = "Post found and returned successfully.";
            s.Responses[404] = "Post with specified ID could not be found.";
        });
        Tags("Users");
        Description(b => b
            .Accepts<GetUserByIdRequest>()
            .Produces<UserRecord>(200, "application/json")
            .ProducesProblem(404)
        );
    }

    public override async Task<Results<Ok<UserRecord>, NotFound, ProblemHttpResult>> ExecuteAsync(
        GetUserByIdRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(UserId.From(request.UserId));
        var result = await mediator.Send(query, cancellationToken);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}
