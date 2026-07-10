using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Core.UserAggregate.Events;
using PetitesVictoires.UseCases.Users.Create;

namespace PetitesVictoires.UnitTests.UseCases.Users.Create;

[TestFixture]
public class SendWelcomeEmailOnUserCreatedTests
{
    private static readonly UserId CreatedUserId = UserId.From(1);

    [SetUp]
    public void SetUp()
    {
        _emailSender = Substitute.For<IEmailSender>();
        _handler = new SendWelcomeEmailOnUserCreated(_emailSender);
    }

    private IEmailSender _emailSender = null!;
    private SendWelcomeEmailOnUserCreated _handler = null!;

    private static UserCreatedEvent Event(string email = "user@example.com", string name = "Thibault")
    {
        var user = new User(CreatedUserId, Email.From(email), UserName.From(name));
        return new UserCreatedEvent(user);
    }

    [Test]
    public async Task Handle_SendsExactlyOneEmailToTheUsersAddress()
    {
        const string recipient = "new@example.com";

        await _handler.Handle(Event(recipient), CancellationToken.None);

        await _emailSender.Received(1).SendEmailAsync(
            recipient,
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_SendsFromTheNoReplyAddress()
    {
        await _handler.Handle(Event(), CancellationToken.None);

        await _emailSender.Received(1).SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_SendsANonEmptySubject()
    {
        await _handler.Handle(Event(), CancellationToken.None);

        await _emailSender.Received(1).SendEmailAsync(
            Arg.Any<string>(),
            Arg.Is<string>(subject => !string.IsNullOrWhiteSpace(subject)),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_GreetsTheUserByNameInTheBody()
    {
        const string name = "Thibault";

        await _handler.Handle(Event(name: name), CancellationToken.None);

        await _emailSender.Received(1).SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Is<string>(body => body.Contains(name)),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_ForwardsTheCancellationToken()
    {
        using var cts = new CancellationTokenSource();

        await _handler.Handle(Event(), cts.Token);

        await _emailSender.Received(1).SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            cts.Token);
    }
}
