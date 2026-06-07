using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetitesVictoires.Infrastructure.Data;

namespace PetitesVictoires.Infrastructure;

public static class InfrastructureServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureServices(ConfigurationManager config, ILogger logger)
        {
            var connectionString = config.GetConnectionString("petitesvictoires");
            Guard.Against.Null(connectionString);

            services.AddScoped<EventDispatchInterceptor>();
            services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

            services.AddDbContext<PetitesVictoiresDbContext>((provider, options) =>
            {
                var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();
                options.UseNpgsql(connectionString);
                options.AddInterceptors(eventDispatchInterceptor);
            });

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
                .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

            logger.LogInformation("{Project} services registered", "Infrastructure");

            return services;
        }
    }
}
