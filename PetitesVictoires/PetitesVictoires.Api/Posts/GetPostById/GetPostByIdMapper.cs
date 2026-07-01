using FastEndpoints;
using PetitesVictoires.UseCases.Posts;

namespace PetitesVictoires.Api.Posts.GetPostById;

public sealed class GetPostByIdMapper : Mapper<GetPostByIdRequest, PostRecord, PostDto>
{
    public override PostRecord FromEntity(PostDto postEntity)
    {
        return new PostRecord(
            postEntity.Id.Value,
            postEntity.Content.Value,
            postEntity.CreatedAt
        );
    }
}
