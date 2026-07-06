using Ardalis.Specification;
using PetitesVictoires.Core.UserAggregate;

namespace PetitesVictoires.Core.PostAggregate.Specifications;

public class PostByIdAndUserSpecification : Specification<Post>
{
    public PostByIdAndUserSpecification(PostId postId, UserId userId)
    {
        Query.Where(p => p.Id == postId && p.UserId == userId);
    }
}
