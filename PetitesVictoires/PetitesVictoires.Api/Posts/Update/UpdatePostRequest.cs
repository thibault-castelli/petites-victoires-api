namespace PetitesVictoires.Api.Posts.Update;

public record UpdatePostRequest
{
    public const string Route = "Posts/{PostId:int}";

    public int PostId { get; init; }
    public required string PostContent { get; init; }

    public static string BuildRoute(int postId)
    {
        return Route.Replace("{PostId:int}", postId.ToString());
    }
}
