using Ardalis.Result;
using Mediator;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.UseCases.Posts.Get;

public record GetPostQuery(PostId PostId) : IQuery<Result<PostDto>>;
