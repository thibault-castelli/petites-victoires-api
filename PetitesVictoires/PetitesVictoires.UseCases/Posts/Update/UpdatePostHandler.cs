using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.PostAggregate.Specifications;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Posts.Update;

public class UpdatePostHandler(IRepository<Post> postRepository, IReadRepository<User> userRepository)
    : ICommandHandler<UpdatePostCommand, Result<PostDto>>
{
    public async ValueTask<Result<PostDto>> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
    {
        var postToUpdate = await postRepository.FirstOrDefaultAsync(
            new PostByIdAndUserSpecification(command.PostId, command.UserId),
            cancellationToken
        );
        if (postToUpdate is null)
        {
            var postOwnedByAnotherUser = await postRepository.GetByIdAsync(command.PostId, cancellationToken);
            return postOwnedByAnotherUser is null
                ? Result.NotFound("Post not found")
                : Result.Forbidden("You cannot update this post");
        }

        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null) return Result.NotFound("User not found");

        postToUpdate.UpdateContent(command.PostContent);
        postToUpdate.MarkUpdated();

        await postRepository.UpdateAsync(postToUpdate, cancellationToken);

        return new PostDto(postToUpdate.Id, command.PostContent, user.Id, user.EmailAddress, user.Name,
            postToUpdate.CreatedAt);
    }
}
