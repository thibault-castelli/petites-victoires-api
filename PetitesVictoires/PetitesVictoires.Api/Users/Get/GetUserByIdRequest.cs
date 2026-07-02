namespace PetitesVictoires.Api.Users.Get;

public record GetUserByIdRequest
{
    public const string Route = "/Users/{UserId:int}";

    public int UserId { get; init; }

    public static string BuildRoute(int userId)
    {
        return Route.Replace("{UserId:int}", userId.ToString());
    }
}