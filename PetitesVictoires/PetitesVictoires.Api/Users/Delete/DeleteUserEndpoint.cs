using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Api.PreProcessors;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Delete;

namespace PetitesVictoires.Api.Users.Delete;

public class DeleteUserEndpoint(IMediator mediator)
    : Endpoint<DeleteUserRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeleteUserRequest.Route);
        PreProcessor<EnsureSelfPreProcessor<DeleteUserRequest>>();
        Summary(s =>
        {
            s.Summary = "Deletes a signed in user";
            s.ExampleRequest = new DeleteUserRequest { UserId = 1 };
            s.Responses[204] = "User deleted successfully";
            s.Responses[400] = "Invalid data";
            s.Responses[401] = "Unauthorized, user not signed in";
            s.Responses[403] = "Forbidden, trying to delete another user";
            s.Responses[404] = "User with given ID not found";
            s.Responses[500] = "Internal server error";
        });
        Tags("Users");
        Description(b => b
            .Accepts<DeleteUserRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
        );
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>> ExecuteAsync(DeleteUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(UserId.From(request.UserId));
        var result = await mediator.Send(command, cancellationToken);

        return result.ToDeleteResult();
    }
}
