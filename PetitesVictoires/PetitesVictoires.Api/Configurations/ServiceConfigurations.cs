using PetitesVictoires.Infrastructure;

namespace PetitesVictoires.Api.Configurations;

public static class ServiceConfigurations
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServiceConfigurations(ILogger logger, WebApplicationBuilder builder)
        {
            services.AddInfrastructureServices(builder.Configuration, logger);

            logger.LogInformation("{Project} services registered", "Infrastructure");

            return services;
        }
    }
}