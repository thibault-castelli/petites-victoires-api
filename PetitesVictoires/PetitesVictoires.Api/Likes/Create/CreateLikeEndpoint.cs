using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Likes.Create;

namespace PetitesVictoires.Api.Likes.Create;

public class CreateLikeEndpoint(IMediator mediator)
    : Endpoint<CreateLikeRequest, Results<Created<LikeRecord>, ValidationProblem, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(CreateLikeRequest.Route);
        Summary(s =>
        {
            s.Summary = "Creates a like on a post";
            s.ExampleRequest = new CreateLikeRequest { PostId = 1 };
            s.ResponseExamples[201] = new LikeRecord(1);
            s.Responses[201] = "Like created successfully";
            s.Responses[400] = "Invalid input data (validation errors)";
            s.Responses[401] = "Unauthorized, user not signed in";
            s.Responses[404] = "Post to like not found";
            s.Responses[409] = "The user has already liked this post";
            s.Responses[500] = "Internal server error";
        });
        Tags("Likes");
        Description(b => b
            .WithName("CreateLike")
            .Accepts<CreateLikeRequest>("application/json")
            .Produces<LikeRecord>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(409)
            .ProducesProblem(500)
        );
    }

    public override async Task<Results<Created<LikeRecord>, ValidationProblem, ProblemHttpResult>> ExecuteAsync(
        CreateLikeRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateLikeCommand(UserId.From(User.GetAuthenticatedUserId()), PostId.From(request.PostId));
        var result = await mediator.Send(command, cancellationToken);

        return result.ToCreatedResult(id => "", id => new LikeRecord(id.Value));
    }
}
