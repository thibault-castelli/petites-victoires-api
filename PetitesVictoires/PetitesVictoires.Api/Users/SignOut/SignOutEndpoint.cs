using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PetitesVictoires.Api.Users.SignOut;

public class SignOutEndpoint : EndpointWithoutRequest<Results<NoContent, ProblemHttpResult>>
{
    public override void Configure()
    {
        Get("/Users/Sign-Out");
        EnableAntiforgery();
        Summary(s =>
        {
            s.Summary = "Sign out a user";
            s.Responses[204] = "Success";
            s.Responses[500] = "Internal server error";
        });
        Tags("Users");
        Description(b => b
            .Produces<NoContent>()
            .ProducesProblem(500));
    }

    public override async Task<Results<NoContent, ProblemHttpResult>> ExecuteAsync(CancellationToken cancellationToken)
    {
        await CookieAuth.SignOutAsync();
        return TypedResults.NoContent();
    }
}
