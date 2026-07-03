namespace PetitesVictoires.Api.Likes.Delete;

public record DeleteLikeRequest
{
    public const string Route = "/Likes";

    public int PostId { get; init; }
}
