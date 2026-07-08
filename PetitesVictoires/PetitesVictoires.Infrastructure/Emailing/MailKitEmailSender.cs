using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PetitesVictoires.Core.Interfaces;

namespace PetitesVictoires.Infrastructure.Emailing;

public class MailKitEmailSender(ILogger<MailKitEmailSender> logger, IOptions<MailSettings> settings) : IEmailSender
{
    public async Task SendEmailAsync(string to, string from, string subject, string body,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(string.IsNullOrWhiteSpace(from) ? settings.Value.From : from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        var security = settings.Value.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        await client.ConnectAsync(settings.Value.Host, settings.Value.Port, security, cancellationToken);

        if (!string.IsNullOrWhiteSpace(settings.Value.UserName) && !string.IsNullOrWhiteSpace(settings.Value.Password))
            await client.AuthenticateAsync(settings.Value.UserName, settings.Value.Password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Email sent to {To} with subject {Subject}", to, subject);
    }
}