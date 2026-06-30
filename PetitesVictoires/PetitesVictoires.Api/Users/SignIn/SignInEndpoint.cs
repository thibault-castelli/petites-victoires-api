using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using FastEndpoints.Security;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Core.Common;
using PetitesVictoires.UseCases.Users.SignIn;

namespace PetitesVictoires.Api.Users.SignIn;

public class SignInEndpoint(IMediator mediator)
    : Endpoint<SignInRequest, Results<NoContent, ValidationProblem, UnauthorizedHttpResult, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(SignInRequest.Route);
        AllowAnonymous();
        EnableAntiforgery();
        Summary(s =>
        {
            s.Summary = "Sign in a user";
            s.Description = "Sign in a user with the provided email and password";
            s.ExampleRequest = new SignInRequest
                { EmailAddress = "example@mail.com", Password = "password", RememberMe = true };
            s.Responses[204] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Forbidden";
            s.Responses[500] = "Internal server error";
        });
        Tags("Users");
        Description(b => b
            .Accepts<SignInRequest>("application/json")
            .Produces<NoContent>()
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, ValidationProblem, UnauthorizedHttpResult, ProblemHttpResult>>
        ExecuteAsync(SignInRequest request, CancellationToken cancellationToken)
    {
        var result =
            await mediator.Send(
                new SignInCommand(Email.From(request.EmailAddress), request.Password), cancellationToken);

        if (!result.IsSuccess)
            return TypedResults.Problem(
                title: "Sign in failed",
                detail: string.Join("; ", result.Errors),
                statusCode: result.Status == ResultStatus.Unauthorized ? 401 : 400
            );

        await CookieAuth.SignInAsync(u =>
        {
            u.Claims.Add(new Claim(ClaimTypes.NameIdentifier, result.Value.Id.ToString()));
            u.Claims.Add(new Claim(ClaimTypes.Email, result.Value.Email));
            u.Claims.Add(new Claim(ClaimTypes.Name, result.Value.UserName));
        }, p => p.IsPersistent = request.RememberMe);

        return TypedResults.NoContent();
    }
}
