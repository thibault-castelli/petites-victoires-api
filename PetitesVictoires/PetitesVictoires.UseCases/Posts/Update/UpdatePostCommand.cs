using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Posts.Update;

public record UpdatePostCommand(PostId PostId, PostContent PostContent, UserId UserId) : ICommand<Result<PostDto>>;
