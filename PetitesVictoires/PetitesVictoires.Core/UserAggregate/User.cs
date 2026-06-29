using Ardalis.SharedKernel;
using PetitesVictoires.Core.Common;

namespace PetitesVictoires.Core.UserAggregate;

public class User : BaseEntity<UserId>, IAggregateRoot
{
    private User()
    {
    } // for EF Core

    public User(UserName name)
    {
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public UserName Name { get; private set; }

    public User UpdateUserName(UserName newName)
    {
        if (Name == newName) return this;

        Name = newName;
        MarkUpdated();
        return this;
    }
}
