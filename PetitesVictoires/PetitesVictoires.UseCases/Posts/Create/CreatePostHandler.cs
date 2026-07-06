using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Posts.Create;

public class CreatePostHandler(IRepository<Post> postRepository, IReadRepository<User> userRepository)
    : ICommandHandler<CreatePostCommand, Result<PostDto>>
{
    public async ValueTask<Result<PostDto>> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var newPost = new Post(command.Content, command.UserId);
        var createdPost = await postRepository.AddAsync(newPost, cancellationToken);

        // User is safe since the endpoint requires users to be signed in
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        return new PostDto(createdPost.Id, createdPost.Content, user!.Id, user.EmailAddress, user.Name,
            createdPost.CreatedAt);
    }
}
