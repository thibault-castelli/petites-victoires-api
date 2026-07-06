using FastEndpoints;
using PetitesVictoires.UseCases.Posts;

namespace PetitesVictoires.Api.Posts.Update;

public sealed class UpdatePostMapper : Mapper<UpdatePostRequest, PostRecord, PostDto>
{
    public override PostRecord FromEntity(PostDto postEntity)
    {
        return new PostRecord(
            postEntity.Id.Value,
            postEntity.Content.Value,
            postEntity.UserId.Value,
            postEntity.UserEmailAddress.Value,
            postEntity.UserName.Value,
            postEntity.CreatedAt
        );
    }
}
