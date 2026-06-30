namespace PetitesVictoires.Api.Users.CreateUser;

public class CreateUserRequest
{
    public const string Route = "/Users";

    public required string EmailAddress { get; init; }
    public required string Name { get; init; }
    public required string Password { get; init; }
}
