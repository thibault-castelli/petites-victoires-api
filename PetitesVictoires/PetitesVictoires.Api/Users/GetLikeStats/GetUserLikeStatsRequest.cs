namespace PetitesVictoires.Api.Users.GetLikeStats;

public record GetUserLikeStatsRequest
{
    public const string Route = "/Users/{UserId:int}/Like-Stats";

    public int UserId { get; init; }

    public static string BuildRoute(int userId)
    {
        return Route.Replace("{UserId:int}", userId.ToString());
    }
}
