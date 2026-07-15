using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Create;

namespace PetitesVictoires.Api.Users.Create;

public class CreateUserEndpoint(IMediator mediator)
    : Endpoint<CreateUserRequest, Results<Created<UserRecord>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateUserRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Creates a new user";
            s.ExampleRequest = new CreateUserRequest
                { EmailAddress = "example@mail.com", Name = "example", Password = "password" };
            s.ResponseExamples[201] = new UserRecord(1, "example@mail.com", "example", DateTime.UtcNow);
            s.Responses[201] = "User created successfully";
            s.Responses[400] = "Invalid input data (validation errors)";
            s.Responses[500] = "Internal server error";
        });
        Tags("Users");
        Description(b => b
            .WithName("CreateUser")
            .Accepts<CreateUserRequest>("application/json")
            .Produces<UserRecord>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500)
        );
    }

    public override async Task<Results<Created<UserRecord>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            Email.From(request.EmailAddress),
            UserName.From(request.Name),
            request.Password
        );
        var result = await mediator.Send(command, cancellationToken);

        return result.ToCreatedResult(id => $"/Users/{id}",
            id => new UserRecord(id.Value, request.EmailAddress, request.Name, DateTime.UtcNow));
    }
}
