using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Posts.Update;

namespace PetitesVictoires.Api.Posts.Update;

public class UpdatePostEndpoint(IMediator mediator)
    : Endpoint<UpdatePostRequest, Results<Ok<PostRecord>, NotFound, ForbidHttpResult, ProblemHttpResult>,
        UpdatePostMapper>
{
    public override void Configure()
    {
        Put(UpdatePostRequest.Route);
        Summary(s =>
        {
            s.Summary = "Update signed in user";
            s.ExampleRequest = new UpdatePostRequest
                { PostId = 1, PostContent = "new post content" };
            s.ResponseExamples[200] =
                new PostRecord(1, "new post content", 1, "example@mail.com", "example", 0, DateTime.UtcNow);
            s.Responses[200] = "Post updated successfully";
            s.Responses[400] = "Invalid input data";
            s.Responses[401] = "Unauthorized, user is not signed in";
            s.Responses[403] = "Forbidden, trying to update a post owned by another user";
            s.Responses[404] = "Post with specified ID not found";
            s.Responses[500] = "Internal server error";
        });
        Tags("Posts");
        Description(b => b
            .Accepts<UpdatePostRequest>("application/json")
            .Produces<PostRecord>(200, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500)
        );
    }

    public override async Task<Results<Ok<PostRecord>, NotFound, ForbidHttpResult, ProblemHttpResult>> ExecuteAsync(
        UpdatePostRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePostCommand(
            PostId.From(request.PostId),
            PostContent.From(request.PostContent),
            UserId.From(User.GetAuthenticatedUserId())
        );
        var result = await mediator.Send(command, cancellationToken);

        return result.ToUpdateResultWithForbidden(Map.FromEntity);
    }
}
