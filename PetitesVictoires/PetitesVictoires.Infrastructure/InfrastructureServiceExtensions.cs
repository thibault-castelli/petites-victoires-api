using Ardalis.GuardClauses;
using Ardalis.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.Infrastructure.Identity;
using PetitesVictoires.Infrastructure.Queries;
using PetitesVictoires.UseCases.Posts.Get;
using PetitesVictoires.UseCases.Posts.List;
using PetitesVictoires.UseCases.Users.List;

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

            services
                .AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<PetitesVictoiresDbContext>()
                .AddSignInManager();

            services
                .AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
                .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
                .AddScoped<IUnitOfWork, EfUnitOfWork>()
                .AddScoped<IIdentityService, IdentityService>();

            services
                .AddScoped<IListUsersQueryService, ListUsersQueryService>()
                .AddScoped<IGetPostQueryService, GetPostQueryService>()
                .AddScoped<IListPostsQueryService, ListPostsQueryService>();

            logger.LogInformation("{Project} services registered", "Infrastructure");

            return services;
        }
    }
}
