using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Users.List;

namespace PetitesVictoires.Api.Users.List;

public class ListUsersEndpoint(IMediator mediator)
    : Endpoint<ListUsersRequest, Results<Ok<ListUsersResponse>, ProblemHttpResult>, ListUsersMapper>
{
    public override void Configure()
    {
        Get(ListUsersRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Lists all users matching the specified criteria and by paging";
            s.ExampleRequest = new ListUsersRequest { Page = 1, CountPerPage = 10 };
            s.ResponseExamples[200] = new ListUsersResponse(
                new List<UserRecord>
                {
                    new(1, "example@mail.com", "example", DateTime.UtcNow),
                    new(2, "example2@mail.com", "example2", DateTime.UtcNow)
                }, 1, 10, 2, 1);
            s.Params["page"] = "1-based page index (default 1)";
            s.Params["count_per_page"] =
                $"Page size from 1 to {Constants.MaxPageSize} (default {Constants.DefaultPageSize})";
            s.Responses[200] = "Paginated list of users returned successfully";
            s.Responses[400] = "Invalid parameters";
        });
        Tags("Users");
        Description(b => b
            .Accepts<ListUsersRequest>()
            .Produces<ListUsersResponse>(200, "application/json")
            .ProducesProblem(400)
        );
    }

    public override async Task<Results<Ok<ListUsersResponse>, ProblemHttpResult>> ExecuteAsync(ListUsersRequest request,
        CancellationToken cancellationToken)
    {
        var query = new ListUsersQuery(request.Page, request.CountPerPage);
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess) return TypedResults.Problem(statusCode: 400, detail: string.Join("; ", result.Errors));

        AddLinkHeader(result.Value.Page, result.Value.CountPerPage, result.Value.TotalPages);

        return result.ToOkOnlyResult(Map.FromEntity);
    }

    private void AddLinkHeader(int page, int countPerPage, int totalPages)
    {
        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

        string Link(string rel, int p)
        {
            return $"<{baseUrl}?page={p}&per_page={countPerPage}>; rel=\"{rel}\"";
        }

        var parts = new List<string>();
        if (page > 1)
        {
            parts.Add(Link("first", 1));
            parts.Add(Link("prev", page - 1));
        }

        if (page < totalPages)
        {
            parts.Add(Link("next", page + 1));
            parts.Add(Link("last", totalPages));
        }

        if (parts.Count > 0)
            HttpContext.Response.Headers["Link"] = string.Join(", ", parts);
    }
}