using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.UseCases.Posts.Get;

public interface IGetPostQueryService
{
    Task<PostDto?> GetPostAsync(PostId postId, CancellationToken cancellationToken);
}
