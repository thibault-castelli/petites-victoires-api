using Ardalis.Result;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.UseCases.Common;

namespace PetitesVictoires.UseCases.Posts.Get;

public record GetPostQuery(PostId PostId) : ICachedQuery<Result<PostDto>>
{
    public string CacheKey => $"post:{PostId.Value}";
    public TimeSpan? CacheTimeout => TimeSpan.FromMinutes(5);
}
