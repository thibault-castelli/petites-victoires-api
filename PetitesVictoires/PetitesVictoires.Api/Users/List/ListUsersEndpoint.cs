using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.Api.Users.List;

public class ListUsersEndpoint(IMediator mediator)
    : Endpoint<ListUsersRequest, Results<Ok<PagedResult<UserRecord>>, ProblemHttpResult>, ListUsersMapper>
{
    public override void Configure()
    {
        Get(ListUsersRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Lists all users matching the specified criteria and by paging";
            s.ExampleRequest = new ListUsersRequest { Page = 1, CountPerPage = 10, Search = "example" };
            s.ResponseExamples[200] = new PagedResult<UserRecord>(
                new List<UserRecord>
                {
                    new(1, "example@mail.com", "example", DateTime.UtcNow),
                    new(2, "example2@mail.com", "example2", DateTime.UtcNow)
                }, 1, 10, 2, 1);
            s.Params["page"] = "1-based page index (default 1)";
            s.Params["count_per_page"] =
                $"Page size from 1 to {Constants.MaxPageSize} (default {Constants.DefaultPageSize})";
            s.Params["search"] = "Filter users by email or username";
            s.Responses[200] = "Paginated list of users returned successfully";
            s.Responses[400] = "Invalid parameters";
        });
        Tags("Users");
        Description(b => b
            .WithName("ListUsers")
            .Accepts<ListUsersRequest>()
            .Produces<PagedResult<UserRecord>>(200, "application/json")
            .ProducesProblem(400)
        );
    }

    public override async Task<Results<Ok<PagedResult<UserRecord>>, ProblemHttpResult>> ExecuteAsync(
        ListUsersRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(Map.ToQuery(request), cancellationToken);
        if (!result.IsSuccess) return TypedResults.Problem(statusCode: 400, detail: string.Join("; ", result.Errors));

        HttpContext.AddLinkHeader(result.Value.Page, result.Value.CountPerPage, result.Value.TotalPages);

        return result.ToOkOnlyResult(Map.FromEntity);
    }
}
