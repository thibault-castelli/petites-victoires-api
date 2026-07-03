using Ardalis.SharedKernel;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Core.LikeAggregate;

public class Like : BaseEntity<LikeId>, IAggregateRoot
{
    private Like()
    {
    } // for EF Core

    public Like(UserId userId, PostId postId)
    {
        UserId = userId;
        PostId = postId;
        CreatedAt = DateTime.UtcNow;
    }

    public UserId UserId { get; private set; }
    public PostId PostId { get; private set; }
}
