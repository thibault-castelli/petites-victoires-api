using Mediator;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate.Events;

namespace PetitesVictoires.UseCases.Users.Create;

public class SendWelcomeEmailOnUserCreated(IEmailSender emailSender)
    : INotificationHandler<UserCreatedEvent>
{
    private const string FromAddress = "no-reply@petitesvictoires.app";

    public async ValueTask Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var user = notification.User;
        const string subject = "Welcom to Petites Victoires!";
        var body =
            $"Hi {user.Name.Value},\n\n" +
            "Welcome to Petites Victoires — your account has been created.\n\n" +
            "See you soon!";

        await emailSender.SendEmailAsync(user.EmailAddress.Value, FromAddress, subject, body, cancellationToken);
    }
}
