using Ardalis.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.Infrastructure.Identity;

namespace PetitesVictoires.Infrastructure.Configuration;

public static class DatabaseConfiguration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDatabaseConfiguration(string connectionString)
        {
            services.AddScoped<EventDispatchInterceptor>();
            services.AddScoped<AuditableInterceptor>();

            services.AddDbContext<PetitesVictoiresDbContext>((provider, options) =>
            {
                var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();
                var auditableInterceptor = provider.GetRequiredService<AuditableInterceptor>();

                options.UseNpgsql(connectionString);
                options.AddInterceptors(eventDispatchInterceptor, auditableInterceptor);
            });

            services
                .AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
                .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
                .AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>()
                .AddScoped<IUnitOfWork, EfUnitOfWork>()
                .AddScoped<IIdentityService, IdentityService>();

            return services;
        }
    }
}
