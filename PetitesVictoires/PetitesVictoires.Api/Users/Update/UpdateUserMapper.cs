using FastEndpoints;
using PetitesVictoires.Api.Users.UpdateUser;
using PetitesVictoires.UseCases.Users;

namespace PetitesVictoires.Api.Users.Update;

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