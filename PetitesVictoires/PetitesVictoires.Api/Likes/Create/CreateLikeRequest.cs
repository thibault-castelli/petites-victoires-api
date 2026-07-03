namespace PetitesVictoires.Api.Likes.Create;

public record CreateLikeRequest
{
    public const string Route = "/Likes";

    public int PostId { get; init; }
}
