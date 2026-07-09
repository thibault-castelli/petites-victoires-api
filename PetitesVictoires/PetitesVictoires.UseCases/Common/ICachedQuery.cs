using Mediator;

namespace PetitesVictoires.UseCases.Common;

public interface ICachedQuery
{
    string CacheKey { get; }
    TimeSpan? CacheTimeout { get; }
}

public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;
