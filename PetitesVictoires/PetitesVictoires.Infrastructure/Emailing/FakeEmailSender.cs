using Microsoft.Extensions.Logging;
using PetitesVictoires.Core.Interfaces;

namespace PetitesVictoires.Infrastructure.Emailing;

public class FakeEmailSender(ILogger<FakeEmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(string to, string subject, string body,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("FAKE email → To: {To}, Subject: {Subject}, Body: {Body}",
            to, subject, body);

        return Task.CompletedTask;
    }
}
