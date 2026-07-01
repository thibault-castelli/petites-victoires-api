using Ardalis.Result;
using Ardalis.SharedKernel;
using Mediator;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Core.UserAggregate.Specifications;

namespace PetitesVictoires.UseCases.Users.Get;

public class GetUserHandler(IReadRepository<User> repository) : IQueryHandler<GetUserQuery, Result<UserDto>>
{
    public async ValueTask<Result<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var specification = new UserByIdSpecification(query.UserId);
        var entity = await repository.FirstOrDefaultAsync(specification, cancellationToken);
        if (entity is null) return Result.NotFound();

        return new UserDto(entity.Id, entity.EmailAddress, entity.Name, entity.CreatedAt);
    }
}
