using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.UseCases.Posts.Get;

namespace PetitesVictoires.Api.Posts.Get;

public class GetPostByIdEndpoint(IMediator mediator)
    : Endpoint<GetPostByIdRequest, Results<Ok<PostRecord>, NotFound, ProblemHttpResult>, GetPostByIdMapper>
{
    public override void Configure()
    {
        Get(GetPostByIdRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Gets a post by ID";
            s.ExampleRequest = new GetPostByIdRequest { PostId = 1 };
            s.ResponseExamples[200] = new PostRecord(1, "Hello World", DateTime.Now);
            s.Responses[200] = "Post found and returned successfully.";
            s.Responses[404] = "Post with specified ID not found.";
        });
        Tags("Posts");
        Description(b => b
            .Accepts<GetPostByIdRequest>()
            .Produces<PostRecord>(200, "application/json")
            .ProducesProblem(404)
        );
    }

    public override async Task<Results<Ok<PostRecord>, NotFound, ProblemHttpResult>> ExecuteAsync(
        GetPostByIdRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetPostQuery(PostId.From(request.PostId));
        var result = await mediator.Send(query, cancellationToken);

        return result.ToGetByIdResult(Map.FromEntity);
    }
}