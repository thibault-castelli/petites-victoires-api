using FastEndpoints;
using PetitesVictoires.UseCases.Posts;

namespace PetitesVictoires.Api.Posts.GetPostById;

public sealed class GetPostByIdMapper : Mapper<GetPostByIdRequest, PostRecord, PostDto>
{
    public override PostRecord FromEntity(PostDto e)
    {
        return new PostRecord(e.Id.Value, e.Content.Value, e.CreatedAt);
    }
}
