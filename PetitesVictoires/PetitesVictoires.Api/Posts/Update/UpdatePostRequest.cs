namespace PetitesVictoires.Api.Posts.Update;

public record UpdatePostRequest
{
    public const string Route = "Posts/{PostId}";

    public int PostId { get; init; }
    public required string PostContent { get; init; }
    public int UserId { get; init; }

    public static string BuildRoute(int userId)
    {
        return Route.Replace("{UserId:int}", userId.ToString());
    }
}
