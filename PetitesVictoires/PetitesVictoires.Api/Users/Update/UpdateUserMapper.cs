using FastEndpoints;
using PetitesVictoires.UseCases.Users;

namespace PetitesVictoires.Api.Users.UpdateUser;

public sealed class UpdateUserMapper : Mapper<UpdateUserRequest, UserRecord, UserDto>
{
    public override UserRecord FromEntity(UserDto userEntity)
    {
        return new UserRecord(
            userEntity.Id.Value,
            userEntity.EmailAddress.Value,
            userEntity.Name.Value,
            userEntity.CreatedAt
        );
    }
}
