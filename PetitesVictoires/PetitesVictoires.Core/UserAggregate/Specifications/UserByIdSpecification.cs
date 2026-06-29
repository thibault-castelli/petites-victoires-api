using Ardalis.Specification;

namespace PetitesVictoires.Core.UserAggregate.Specifications;

public class UserByIdSpecification : Specification<User>
{
    public UserByIdSpecification(UserId userId)
    {
        Query.Where(u => u.Id == userId);
    }
}
