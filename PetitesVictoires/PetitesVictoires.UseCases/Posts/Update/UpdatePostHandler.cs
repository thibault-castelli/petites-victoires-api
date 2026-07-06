using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Posts.Update;

public class UpdatePostHandler(IRepository<Post> postRepository, IReadRepository<User> userRepository)
    : ICommandHandler<UpdatePostCommand, Result<PostDto>>
{
    public async ValueTask<Result<PostDto>> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
    {
        var postToUpdate = await postRepository.GetByIdAsync(command.PostId, cancellationToken);
        if (postToUpdate is null) return Result.NotFound();
        if (postToUpdate.UserId != command.UserId) return Result.Forbidden();

        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null) return Result.NotFound("User not found");

        postToUpdate.UpdateContent(command.PostContent);
        postToUpdate.MarkUpdated();

        await postRepository.UpdateAsync(postToUpdate, cancellationToken);

        return new PostDto(postToUpdate.Id, command.PostContent, user.Id, user.EmailAddress, user.Name,
            postToUpdate.CreatedAt);
    }
}
