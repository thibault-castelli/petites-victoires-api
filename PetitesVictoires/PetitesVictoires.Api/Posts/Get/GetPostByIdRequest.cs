namespace PetitesVictoires.Api.Posts.GetPostById;

public record GetPostByIdRequest
{
    public const string Route = "/Posts/{PostId:int}";

    public int PostId { get; init; }

    public static string BuildRoute(int postId)
    {
        return Route.Replace("{PostId:int}", postId.ToString());
    }
}
