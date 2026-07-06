namespace PetitesVictoires.Api.Posts.Create;

public record CreatePostRequest
{
    public const string Route = "/Posts";

    public required string Content { get; init; }
}
