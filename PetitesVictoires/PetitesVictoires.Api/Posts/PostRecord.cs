namespace PetitesVictoires.Api.Posts;

public record PostRecord(
    int Id,
    string Content,
    int UserId,
    string UserEmailAddress,
    string UserName,
    DateTime CreatedAt
);
