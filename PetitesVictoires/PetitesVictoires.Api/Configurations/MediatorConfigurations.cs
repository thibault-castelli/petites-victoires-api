using Ardalis.SharedKernel;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Infrastructure;
using PetitesVictoires.UseCases.Posts.Get;

namespace PetitesVictoires.Api.Configurations;

public static class MediatorConfigurations
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediatorSourceGen(ILogger logger)
        {
            logger.LogInformation("Registering Mediator SourceGen and Behaviors");
            services.AddMediator(options =>
            {
                // Lifetime: Singleton is fastest per docs; Scoped/Transient also supported.
                options.ServiceLifetime = ServiceLifetime.Scoped;

                // Supply any TYPE from each assembly you want scanned (the generator finds the assembly from the type)
                options.Assemblies =
                [
                    typeof(Post), // Core
                    typeof(GetPostQuery), // UseCases
                    typeof(InfrastructureServiceExtensions), // Infrastructure
                    typeof(MediatorConfigurations) // Web
                ];

                // Register pipeline behaviors here (order matters)
                options.PipelineBehaviors =
                [
                    typeof(LoggingBehavior<,>)
                ];

                // If you have stream behaviors:
                // options.StreamPipelineBehaviors = [ typeof(YourStreamBehavior<,>) ];
            });

            return services;
        }
    }
}
