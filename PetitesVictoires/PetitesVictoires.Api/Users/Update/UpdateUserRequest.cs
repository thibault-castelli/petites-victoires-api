using PetitesVictoires.Api.PreProcessors;

namespace PetitesVictoires.Api.Users.UpdateUser;

public record UpdateUserRequest : IOwnedResource
{
    public const string Route = "/Users/{UserId:int}";
    public required string EmailAddress { get; init; }
    public required string Name { get; init; }

    public int UserId { get; init; }

    public static string BuildRoute(int userId)
    {
        return Route.Replace("{UserId:int}", userId.ToString());
    }
}
