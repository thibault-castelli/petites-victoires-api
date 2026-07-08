using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetitesVictoires.Infrastructure.Configuration;

namespace PetitesVictoires.Infrastructure;

public static class InfrastructureServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(ConfigurationManager config, ILogger logger)
        {
            var connectionString = config.GetConnectionString("petitesvictoires");
            Guard.Against.Null(connectionString);

            services.AddDatabaseConfiguration(connectionString);

            services.AddIdentityServices();

            services.AddEmailConfiguration(config);

            services.AddQueryServicesConfiguration();

            logger.LogInformation("{Project} services registered", "Infrastructure");

            return services;
        }
    }
}
