using Ardalis.SharedKernel;
using PetitesVictoires.Core.Common;

namespace PetitesVictoires.Core.UserAggregate;

public class User : BaseEntity<UserId>, IAggregateRoot
{
    private User()
    {
    } // for EF Core

    public User(UserId id, Email emailAddress, UserName name)
    {
        Id = id;
        EmailAddress = emailAddress;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public Email EmailAddress { get; private set; }
    public UserName Name { get; private set; }

    public User UpdateUserName(UserName newName)
    {
        if (Name == newName) return this;

        Name = newName;
        MarkUpdated();
        return this;
    }
}
