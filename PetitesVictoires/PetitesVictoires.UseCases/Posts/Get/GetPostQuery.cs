using Ardalis.Result;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.UseCases.Behaviors;

namespace PetitesVictoires.UseCases.Posts.Get;

public record GetPostQuery(PostId PostId) : ICachedQuery<Result<PostDto>>
{
    public string CacheKey => $"{Constants.PostCachePrefix}{PostId.Value}";
    public TimeSpan? CacheTimeout => TimeSpan.FromMinutes(5);
}