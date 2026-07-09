using Ardalis.Specification;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.Core.LikeAggregate.Specifications;

public class LikeByPostSpecification : Specification<Like>
{
    public LikeByPostSpecification(PostId postId)
    {
        Query.Where(like => like.PostId == postId);
    }
}
