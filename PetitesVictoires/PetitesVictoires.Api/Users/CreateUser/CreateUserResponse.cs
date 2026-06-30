namespace PetitesVictoires.Api.Users.CreateUser;

public class CreateUserResponse(int id, string emailAddress, string name)
{
    public int Id { get; set; } = id;
    public string EmailAddress { get; set; } = emailAddress;
    public string Name { get; set; } = name;
}
