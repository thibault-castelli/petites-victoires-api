using FastEndpoints;
using PetitesVictoires.Api.Extensions;

namespace PetitesVictoires.Api.PreProcessors;

public sealed class EnsureSelfPreProcessor<TRequest> : IPreProcessor<TRequest> where TRequest : IOwnedResource
{
    public Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
    {
        var isForbidden = context.Request is null ||
                          !context.HttpContext.User.TryGetAuthenticatedUserId(out var authenticatedId) ||
                          authenticatedId != context.Request.UserId;
        if (isForbidden) return context.HttpContext.Response.SendForbiddenAsync(ct);

        return Task.CompletedTask;
    }
}
