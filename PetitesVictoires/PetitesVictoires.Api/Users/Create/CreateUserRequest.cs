namespace PetitesVictoires.Api.Users.Create;

public record CreateUserRequest
{
    public const string Route = "/Users";

    public required string EmailAddress { get; init; }
    public required string Name { get; init; }
    public required string Password { get; init; }
}