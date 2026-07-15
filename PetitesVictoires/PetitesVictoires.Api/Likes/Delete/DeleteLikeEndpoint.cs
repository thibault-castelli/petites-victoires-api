using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Likes.Delete;

namespace PetitesVictoires.Api.Likes.Delete;

public class DeleteLikeEndpoint(IMediator mediator)
    : Endpoint<DeleteLikeRequest, Results<NoContent, NotFound, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeleteLikeRequest.Route);
        Summary(s =>
        {
            s.Summary = "Deletes a like";
            s.ExampleRequest = new DeleteLikeRequest { PostId = 1 };
            s.Responses[204] = "Like deleted successfully";
            s.Responses[401] = "Unauthorized, user not signed in";
            s.Responses[404] = "Like not found";
            s.Responses[500] = "Internal server error";
        });
        Tags("Likes");
        Description(b => b
            .WithName("DeleteLike")
            .Accepts<DeleteLikeRequest>("application/json")
            .Produces<NoContent>()
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(500)
        );
    }

    public override async Task<Results<NoContent, NotFound, ProblemHttpResult>> ExecuteAsync(DeleteLikeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DeleteLikeCommand(UserId.From(User.GetAuthenticatedUserId()), PostId.From(request.PostId));
        var result = await mediator.Send(command, cancellationToken);

        return result.ToDeleteResult();
    }
}
