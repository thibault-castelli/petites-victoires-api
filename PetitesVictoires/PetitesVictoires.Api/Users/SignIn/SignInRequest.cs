namespace PetitesVictoires.Api.Users.SignIn;

public class SignInRequest
{
    public const string Route = "Users/Sign-In";

    public required string EmailAddress { get; init; }
    public required string Password { get; init; }
    public bool RememberMe { get; init; }
}
