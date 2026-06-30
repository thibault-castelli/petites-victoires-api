using PetitesVictoires.Infrastructure;

namespace PetitesVictoires.Api.Configurations;

public static class ServiceConfigurations
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServiceConfigurations(ILogger logger, WebApplicationBuilder builder)
        {
            services.AddInfrastructureServices(builder.Configuration, logger)
                .AddMediatorSourceGen(logger)
                .AddIdentityConfigurations(logger)
                .AddFastEndpointsConfigurations(logger);

            return services;
        }
    }
}
