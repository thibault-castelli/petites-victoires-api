using Ardalis.SharedKernel;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate.Events;

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

        RegisterDomainEvent(new UserCreatedEvent(this));
    }

    public Email EmailAddress { get; private set; }
    public UserName Name { get; private set; }

    public User UpdateEmailAddress(Email emailAddress)
    {
        if (EmailAddress == emailAddress) return this;
        EmailAddress = emailAddress;
        return this;
    }

    public User UpdateName(UserName newName)
    {
        if (Name == newName) return this;
        Name = newName;
        return this;
    }
}
