using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;

namespace PetitesVictoires.Api.Configurations;

public static class IdentityConfigurations
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddIdentityConfigurations(ILogger logger)
        {
            logger.LogInformation("Adding Identity Configurations");

            services.AddAuthenticationCookie(
                TimeSpan.FromMinutes(60),
                options => // optional configuration overload
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Events.OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = ctx =>
                    {
                        ctx.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };
                });
            services.AddAuthorization();
            services.AddAntiforgery();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            return services;
        }
    }
}
