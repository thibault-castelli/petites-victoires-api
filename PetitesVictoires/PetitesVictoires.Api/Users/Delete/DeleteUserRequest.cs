using PetitesVictoires.Api.PreProcessors;

namespace PetitesVictoires.Api.Users.Delete;

public record DeleteUserRequest : IOwnedResource
{
    public const string Route = "/Users/{UserId:int}";

    public int UserId { get; init; }

    public static string BuildRoute(int userId)
    {
        return Route.Replace("{UserId:int}", userId.ToString());
    }
}
