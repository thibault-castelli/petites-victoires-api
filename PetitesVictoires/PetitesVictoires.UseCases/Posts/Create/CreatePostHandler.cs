using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.UseCases.Posts.Create;

public class CreatePostHandler(IRepository<Post> repository) : ICommandHandler<CreatePostCommand, Result<PostId>>
{
    public async ValueTask<Result<PostId>> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var newPost = new Post(command.Content, command.UserId);
        var createdPost = await repository.AddAsync(newPost, cancellationToken);

        return createdPost.Id;
    }
}
