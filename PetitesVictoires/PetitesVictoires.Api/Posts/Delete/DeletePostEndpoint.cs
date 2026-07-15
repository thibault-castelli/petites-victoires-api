using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Posts.Delete;

namespace PetitesVictoires.Api.Posts.Delete;

public class DeletePostEndpoint(IMediator mediator)
    : Endpoint<DeletePostRequest, Results<NoContent, NotFound, ForbidHttpResult, ProblemHttpResult>>
{
    public override void Configure()
    {
        Delete(DeletePostRequest.Route);
        Summary(s =>
        {
            s.Summary = "Soft deletes a post";
            s.ExampleRequest = new DeletePostRequest { PostId = 1 };
            s.Responses[204] = "Post soft deleted successfully";
            s.Responses[400] = "Invalid data";
            s.Responses[401] = "Unauthorized, user not signed in";
            s.Responses[403] = "Forbidden, trying to delete a post owned by another user";
            s.Responses[404] = "Post with given ID not found";
            s.Responses[500] = "Internal server error";
        });
        Tags("Posts");
        Description(b => b
            .WithName("DeletePost")
            .Accepts<DeletePostRequest>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
        );
    }

    public override async Task<Results<NoContent, NotFound, ForbidHttpResult, ProblemHttpResult>> ExecuteAsync(
        DeletePostRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DeletePostCommand(PostId.From(request.PostId), UserId.From(User.GetAuthenticatedUserId()));
        var result = await mediator.Send(command, cancellationToken);

        return result.ToDeleteWithForbidResult();
    }
}
