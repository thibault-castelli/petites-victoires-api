using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Posts.Delete;

public record DeletePostCommand(PostId PostId, UserId UserId) : ICommand<Result>;
