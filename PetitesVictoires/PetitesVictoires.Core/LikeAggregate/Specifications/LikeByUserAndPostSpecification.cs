using Ardalis.Specification;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Core.LikeAggregate.Specifications;

public class LikeByUserAndPostSpecification : Specification<Like>
{
    public LikeByUserAndPostSpecification(UserId userId, PostId postId)
    {
        Query.Where(l => l.UserId == userId && l.PostId == postId);
    }
}
