using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.UseCases.Posts;

public record PostDto(
    PostId Id,
    PostContent Content,
    UserId UserId,
    Email UserEmailAddress,
    UserName UserName,
    DateTime CreatedAt,
    int LikesCount = 0
);
