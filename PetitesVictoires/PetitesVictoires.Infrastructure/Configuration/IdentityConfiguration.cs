using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.Infrastructure.Identity;

namespace PetitesVictoires.Infrastructure.Configuration;

public static class IdentityConfiguration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddIdentityServices()
        {
            services
                .AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<PetitesVictoiresDbContext>()
                .AddSignInManager();

            return services;
        }
    }
}
