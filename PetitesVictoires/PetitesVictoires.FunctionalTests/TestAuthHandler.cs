using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PetitesVictoires.FunctionalTests;

/// <summary>
///     Authenticates a request as whatever user id the test puts in the X-Test-UserId header, so authed
///     endpoints can be driven without the real cookie + antiforgery sign-in every time.
///     With no header it defers to the app's real cookie scheme, which keeps the genuine sign-in flow
///     testable end-to-end (and leaves unauthenticated requests anonymous -> 401).
/// </summary>
public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    public const string UserIdHeader = "X-Test-UserId";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserIdHeader, out var userId))
            return await Context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, SchemeName);
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName));
    }
}
