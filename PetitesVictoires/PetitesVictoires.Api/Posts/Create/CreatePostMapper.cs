using FastEndpoints;
using PetitesVictoires.UseCases.Posts;

namespace PetitesVictoires.Api.Posts.Create;

public class CreatePostMapper : Mapper<CreatePostRequest, PostRecord, PostDto>
{
    public override PostRecord FromEntity(PostDto postEntity)
    {
        return new PostRecord(
            postEntity.Id.Value,
            postEntity.Content.Value,
            postEntity.UserId.Value,
            postEntity.UserEmailAddress.Value,
            postEntity.UserName.Value,
            postEntity.LikesCount,
            postEntity.CreatedAt
        );
    }
}
