using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Create;

namespace PetitesVictoires.Api.Users.CreateUser;

public class CreateUserEndpoint(IMediator mediator)
    : Endpoint<CreateUserRequest, Results<Created<CreatedUserResponse>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateUserRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Creates a new user";
            s.Description = "Creates a new user with the provided email address, name and password.";
            s.ExampleRequest = new CreateUserRequest
                { EmailAddress = "example@mail.com", Name = "example", Password = "password" };
            s.ResponseExamples[201] = new CreatedUserResponse(1, "example@mail.com", "example");

            s.Responses[201] = "User created successfully";
            s.Responses[400] = "Invalid input data (validation errors";
            s.Responses[500] = "Internal server error";
        });
        Tags("Users");
        Description(builder => builder
            .Accepts<CreateUserRequest>("application/json")
            .Produces<CreatedUserResponse>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(500));
    }

    public override async Task<Results<Created<CreatedUserResponse>, ValidationProblem, ProblemHttpResult>>
        ExecuteAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateUserCommand(Email.From(request.EmailAddress),
            UserName.From(request.Name), request.Password), cancellationToken);

        return result.ToCreatedResult(id => $"/Users/{id}",
            id => new CreatedUserResponse(id.Value, request.EmailAddress, request.Name));
    }
}
