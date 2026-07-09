using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts.List;

namespace PetitesVictoires.Api.Posts.List;

public class ListPostsEndpoint(IMediator mediator)
    : Endpoint<ListPostsRequest, Results<Ok<ListPostsResponse>, ProblemHttpResult>, ListPostsMapper>
{
    public override void Configure()
    {
        Get(ListPostsRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Lists all posts matching the specified criteria and by paging";
            s.ExampleRequest = new ListPostsRequest { Page = 1, CountPerPage = 10 };
            s.ResponseExamples[200] = new ListPostsResponse(
                new List<PostRecord>
                {
                    new(1, "lorem ipsum", 1, "example@mail.com", "example", 0, DateTime.UtcNow),
                    new(2, "example content", 2, "example2@mail.com", "example2", 10, DateTime.UtcNow)
                }, 1, 10, 2, 1);
            s.Params["page"] = "1-based page index (default 1)";
            s.Params["count_per_page"] =
                $"Page size from 1 to {Constants.MaxPageSize} (default {Constants.DefaultPageSize})";
            s.Responses[200] = "Paginated list of users returned successfully";
            s.Responses[400] = "Invalid parameters";
        });
        Tags("Posts");
        Description(b => b
            .Accepts<ListPostsRequest>()
            .Produces<ListPostsResponse>(200, "application/json")
            .ProducesProblem(400)
        );
    }

    public override async Task<Results<Ok<ListPostsResponse>, ProblemHttpResult>> ExecuteAsync(ListPostsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new ListPostsQuery(request.Page, request.CountPerPage);
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess) return TypedResults.Problem(statusCode: 400, detail: string.Join("; ", result.Errors));

        HttpContext.AddLinkHeader(result.Value.Page, result.Value.CountPerPage, result.Value.TotalPages);

        return result.ToOkOnlyResult(Map.FromEntity);
    }
}
