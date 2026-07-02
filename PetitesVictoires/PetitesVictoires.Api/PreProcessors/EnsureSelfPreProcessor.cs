using System.Security.Claims;
using FastEndpoints;

namespace PetitesVictoires.Api.PreProcessors;

public sealed class EnsureSelfPreProcessor<TRequest> : IPreProcessor<TRequest> where TRequest : IOwnedResource
{
    public Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
    {
        var claimId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isForbidden = context.Request is null || !int.TryParse(claimId, out var authenticatedId) ||
                          authenticatedId != context.Request.UserId;
        if (isForbidden) return context.HttpContext.Response.SendForbiddenAsync(ct);

        return Task.CompletedTask;
    }
}
