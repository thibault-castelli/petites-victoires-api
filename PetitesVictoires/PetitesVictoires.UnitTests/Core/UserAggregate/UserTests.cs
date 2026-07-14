using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Core.UserAggregate.Events;
using Shouldly;

namespace PetitesVictoires.UnitTests.Core.UserAggregate;

[TestFixture]
public class UserTests
{
    private static readonly UserId Id = UserId.From(1);

    private static User CreateUser(string email = "user@example.com", string name = "Thibault")
    {
        return new User(Id, Email.From(email), UserName.From(name));
    }

    [Test]
    public void Constructor_SetsIdEmailAndName()
    {
        var email = Email.From("user@example.com");
        var name = UserName.From("Thibault");

        var user = new User(Id, email, name);

        user.Id.ShouldBe(Id);
        user.EmailAddress.ShouldBe(email);
        user.Name.ShouldBe(name);
    }

    [Test]
    public void Constructor_StampsCreatedAtWithUtcNow()
    {
        var before = DateTime.UtcNow;

        var user = CreateUser();

        user.CreatedAt.ShouldBeInRange(before, DateTime.UtcNow);
    }

    [Test]
    public void Constructor_RegistersASingleUserCreatedEvent()
    {
        var user = CreateUser();

        user.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<UserCreatedEvent>();
    }

    [Test]
    public void Constructor_RaisesUserCreatedEventCarryingTheUserItself()
    {
        var user = CreateUser();

        var domainEvent = user.DomainEvents.OfType<UserCreatedEvent>().Single();
        domainEvent.User.ShouldBeSameAs(user);
    }

    [Test]
    public void UpdateEmailAddress_WithDifferentEmail_ReplacesTheEmail()
    {
        var user = CreateUser(email: "old@example.com");
        var newEmail = Email.From("new@example.com");

        user.UpdateEmailAddress(newEmail);

        user.EmailAddress.ShouldBe(newEmail);
    }

    [Test]
    public void UpdateEmailAddress_ReturnsTheSameInstance_ForFluentChaining()
    {
        var user = CreateUser(email: "old@example.com");

        var result = user.UpdateEmailAddress(Email.From("new@example.com"));

        result.ShouldBeSameAs(user);
    }

    [Test]
    public void UpdateEmailAddress_WithIdenticalEmail_LeavesEmailUnchanged()
    {
        var user = CreateUser(email: "same@example.com");

        user.UpdateEmailAddress(Email.From("same@example.com"));

        user.EmailAddress.ShouldBe(Email.From("same@example.com"));
    }

    [Test]
    public void UpdateName_WithDifferentName_ReplacesTheName()
    {
        var user = CreateUser(name: "Old");
        var newName = UserName.From("New");

        user.UpdateName(newName);

        user.Name.ShouldBe(newName);
    }

    [Test]
    public void UpdateName_ReturnsTheSameInstance_ForFluentChaining()
    {
        var user = CreateUser(name: "Old");

        var result = user.UpdateName(UserName.From("New"));

        result.ShouldBeSameAs(user);
    }

    [Test]
    public void UpdateName_WithIdenticalName_LeavesNameUnchanged()
    {
        var user = CreateUser(name: "Same");

        user.UpdateName(UserName.From("Same"));

        user.Name.ShouldBe(UserName.From("Same"));
    }

    [Test]
    public void NewUser_HasNoUpdatedTimestamp()
    {
        var user = CreateUser();

        user.UpdatedAt.ShouldBeNull();
    }

    [Test]
    public void MarkUpdated_SetsUpdatedAtToUtcNow()
    {
        var user = CreateUser();

        var before = DateTime.UtcNow;
        user.MarkUpdated();

        user.UpdatedAt.ShouldNotBeNull();
        user.UpdatedAt!.Value.ShouldBeInRange(before, DateTime.UtcNow);
    }
}
