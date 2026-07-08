using Ardalis.SharedKernel;

namespace PetitesVictoires.Core.UserAggregate.Events;

public sealed class UserCreatedEvent(User user) : DomainEventBase
{
    public User User { get; } = user;
}
