using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Infrastructure.Emailing;

namespace PetitesVictoires.Infrastructure.Configuration;

public static class EmailConfiguration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEmailConfiguration(ConfigurationManager config)
        {
            services.AddOptions<MailSettings>()
                .Bind(config.GetSection("MailSettings"))
                .PostConfigure(mail =>
                {
                    var mailPit = config.GetConnectionString("mailpit");
                    if (string.IsNullOrWhiteSpace(mailPit)) return;

                    var csb = new DbConnectionStringBuilder { ConnectionString = mailPit };
                    if (!csb.TryGetValue("endpoint", out var endpoint) ||
                        !Uri.TryCreate(endpoint.ToString(), UriKind.Absolute, out var uri)) return;

                    mail.Host = uri.Host;
                    mail.Port = uri.Port;
                    mail.UseSsl = false;
                    mail.UserName = null;
                    mail.Password = null;
                });

            var mailConfigured =
                !string.IsNullOrWhiteSpace(config.GetConnectionString("mailpit")) ||
                !string.IsNullOrWhiteSpace(config[$"{MailSettings.SectionName}:Host"]);

            if (mailConfigured)
                services.AddScoped<IEmailSender, MailKitEmailSender>();
            else
                services.AddScoped<IEmailSender, FakeEmailSender>();

            return services;
        }
    }
}
