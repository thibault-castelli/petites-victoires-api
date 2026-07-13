using System.Text.Json;
using Ardalis.Result;
using Mediator;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace PetitesVictoires.UseCases.Behaviors;

public class CachingBehavior<TMessage, TResponse>(
    IDistributedCache cache,
    ILogger<CachingBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    private readonly TimeSpan _defaultTimeSpan = TimeSpan.FromMinutes(5);

    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (message is not ICachedQuery q) return await next(message, cancellationToken);

        var cached = await cache.GetStringAsync(q.CacheKey, cancellationToken);
        if (cached is not null)
        {
            logger.LogInformation("Cache hit for {CacheKey}", q.CacheKey);
            return JsonSerializer.Deserialize<TResponse>(cached)!;
        }

        var response = await next(message, cancellationToken);
        if (response is not IResult { Status: ResultStatus.Ok }) return response;

        logger.LogInformation("Cache miss, caching {CacheKey}", q.CacheKey);
        await cache.SetStringAsync(
            q.CacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
                { AbsoluteExpirationRelativeToNow = q.CacheTimeout ?? _defaultTimeSpan },
            cancellationToken
        );

        return response;
    }
}
