using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Posts.Create;

public record CreatePostCommand(PostContent Content, UserId UserId) : ICommand<Result<PostId>>;
