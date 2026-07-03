using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Likes.Create;

public record CreateLikeCommand(UserId UserId, PostId PostId) : ICommand<Result<LikeId>>;
