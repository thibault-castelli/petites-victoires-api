using Ardalis.Specification;

namespace PetitesVictoires.Core.PostAggregate.Specifications;

public class PostByIdSpecification : Specification<Post>
{
    public PostByIdSpecification(PostId postId)
    {
        Query.Where(p => p.Id == postId);
    }
}
