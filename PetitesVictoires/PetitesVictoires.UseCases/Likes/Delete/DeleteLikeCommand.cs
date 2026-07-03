using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Likes.Delete;

public record DeleteLikeCommand(UserId UserId, PostId PostId) : ICommand<Result>;
