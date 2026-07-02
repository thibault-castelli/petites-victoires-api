using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Api.PreProcessors;
using PetitesVictoires.Api.Users.UpdateUser;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Update;

namespace PetitesVictoires.Api.Users.Update;

public class UpdateUserEndpoint(IMediator mediator)
    : Endpoint<UpdateUserRequest, Results<Ok<UserRecord>, NotFound, ProblemHttpResult>, UpdateUserMapper>
{
    public override void Configure()
    {
        Put(UpdateUserRequest.Route);
        PreProcessor<EnsureSelfPreProcessor<UpdateUserRequest>>();
        Summary(s =>
        {
            s.Summary = "Update signed in user";
            s.ExampleRequest = new UpdateUserRequest
                { UserId = 1, EmailAddress = "new-example@mail.com", Name = "newName" };
            s.ResponseExamples[200] = new UserRecord(1, "new-example@mail.com", "newName", DateTime.UtcNow);
            s.Responses[200] = "User updated successfully";
            s.Responses[400] = "Invalid input data";
            s.Responses[401] = "Unauthorized, user is not signed in";
            s.Responses[403] = "Forbidden, trying to update another user than self";
            s.Responses[404] = "User with specified ID not found";
            s.Responses[500] = "Internal server error";
        });
        Tags("Users");
        Description(b => b
            .Accepts<UpdateUserRequest>("application/json")
            .Produces<UserRecord>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500)
        );
    }

    public override async Task<Results<Ok<UserRecord>, NotFound, ProblemHttpResult>> ExecuteAsync(
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(
            UserId.From(request.UserId),
            Email.From(request.EmailAddress),
            UserName.From(request.Name),
            request.CurrentPassword,
            request.NewPassword
        );
        var result = await mediator.Send(command, cancellationToken);

        return result.ToUpdateResult(Map.FromEntity);
    }
}
