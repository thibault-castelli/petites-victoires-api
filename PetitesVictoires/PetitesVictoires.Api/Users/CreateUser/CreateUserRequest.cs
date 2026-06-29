namespace PetitesVictoires.Api.Users.CreateUser;

public class CreateUserRequest
{
    public const string Route = "/Users";

    public string EmailAddress { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
}
