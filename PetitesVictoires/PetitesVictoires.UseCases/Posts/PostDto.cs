using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.UseCases.Posts;

public record PostDto(PostId Id, PostContent Content, DateTime CreatedAt);
