using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Core.UserAggregate.Events;
using PetitesVictoires.UseCases.Users.Create;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.Create;

[TestFixture]
public class SendWelcomeEmailOnUserCreatedTests
{
    private const string ExpectedFrom = "no-reply@petitesvictoires.app";

    private IEmailSender _emailSender = null!;
    private SendWelcomeEmailOnUserCreated _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _emailSender = Substitute.For<IEmailSender>();
        _handler = new SendWelcomeEmailOnUserCreated(_emailSender);
    }

    private static UserCreatedEvent Event(string email = "user@example.com", string name = "Thibault")
    {
        var user = new User(UserId.From(1), Email.From(email), UserName.From(name));
        return new UserCreatedEvent(user);
    }

    [Test]
    public async Task Handle_SendsExactlyOneEmailToTheUsersAddress()
    {
        await _handler.Handle(Event(email: "new@example.com"), CancellationToken.None);

        await _emailSender.Received(1).SendEmailAsync(
            "new@example.com",
            Arg.Any<string>(),
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
            ExpectedFrom,
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
            Arg.Any<string>(),
            Arg.Is<string>(subject => !string.IsNullOrWhiteSpace(subject)),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_GreetsTheUserByNameInTheBody()
    {
        await _handler.Handle(Event(name: "Thibault"), CancellationToken.None);

        await _emailSender.Received(1).SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Is<string>(body => body.Contains("Thibault")),
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
            Arg.Any<string>(),
            cts.Token);
    }
}
