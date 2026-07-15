using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Posts.Create;

namespace PetitesVictoires.Api.Posts.Create;

public class CreatePostEndpoint(IMediator mediator)
    : Endpoint<CreatePostRequest, Results<Created<PostRecord>, ValidationProblem, ProblemHttpResult>, CreatePostMapper>
{
    public override void Configure()
    {
        Post(CreatePostRequest.Route);
        Summary(s =>
        {
            s.Summary = "Creates a post";
            s.ExampleRequest = new CreatePostRequest { Content = "example content" };
            s.ResponseExamples[201] = PostId.From(1);
            s.Responses[201] = "Post created successfully";
            s.Responses[400] = "Invalid input data (validation errors)";
            s.Responses[401] = "Unauthorized, user not signed in";
            s.Responses[500] = "Internal server error";
        });
        Tags("Posts");
        Description(b => b
            .WithName("CreatePost")
            .Accepts<CreatePostRequest>("application/json")
            .Produces<PostId>(201, "application/json")
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500)
        );
    }

    public override async Task<Results<Created<PostRecord>, ValidationProblem, ProblemHttpResult>> ExecuteAsync(
        CreatePostRequest request, CancellationToken cancellationToken)
    {
        var command =
            new CreatePostCommand(PostContent.From(request.Content), UserId.From(User.GetAuthenticatedUserId()));
        var result = await mediator.Send(command, cancellationToken);

        return result.ToCreatedResult(p => $"/Posts/{p.Id.Value}", Map.FromEntity);
    }
}
