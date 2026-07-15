using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using PetitesVictoires.Core.Interfaces;

namespace PetitesVictoires.FunctionalTests;

/// <summary>
///     Boots the real app against the test container.
///     Note connection strings and the environment are NOT set here: Program.cs consumes them while
///     registering services, which runs before these callbacks. DatabaseFixture puts them in the
///     environment before the host boots. Everything below replaces already-registered services, which
///     ConfigureTestServices can do because it runs last.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // No Redis in tests; an always-miss cache also keeps list endpoints deterministic.
            services.RemoveAll<IDistributedCache>();
            services.AddSingleton<IDistributedCache, NoOpDistributedCache>();

            // appsettings.json ships a real MailSettings:Host, so the app picks MailKitEmailSender.
            // Creating a user raises UserCreatedEvent -> welcome email, which would dial a real SMTP host.
            services.RemoveAll<IEmailSender>();
            services.AddScoped(_ => Substitute.For<IEmailSender>());

            // SignIn/SignOut opt into antiforgery, but the app exposes no token endpoint — no-op the check.
            services.RemoveAll<IAntiforgery>();
            services.AddSingleton(_ =>
            {
                var antiforgery = Substitute.For<IAntiforgery>();
                antiforgery.ValidateRequestAsync(Arg.Any<HttpContext>()).Returns(Task.CompletedTask);
                return antiforgery;
            });

            // Only authenticate/challenge are redirected to the test handler; DefaultScheme stays on
            // cookies so CookieAuth.SignInAsync/SignOutAsync still work in the sign-in tests.
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }
}
