using FastEndpoints;
using FastEndpoints.Swagger;

namespace PetitesVictoires.Api.Configurations;

public static class FastEndpointsConfiguration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddFastEndpointsConfigurations(ILogger logger)
        {
            logger.LogInformation("Adding Fast Endpoints Configurations");

            services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                    o.DocumentSettings = s =>
                    {
                        s.Title = "Petites Victoires API";
                        s.Version = "v1";
                        s.Description = "Http endpoints for the Petites Victoires API";
                    };
                    o.ShortSchemaNames = true;
                });

            return services;
        }
    }
}
