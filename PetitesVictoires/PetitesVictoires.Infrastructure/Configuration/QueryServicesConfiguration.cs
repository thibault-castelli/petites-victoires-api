using Microsoft.Extensions.DependencyInjection;
using PetitesVictoires.Infrastructure.Queries;
using PetitesVictoires.UseCases.Posts.Get;
using PetitesVictoires.UseCases.Posts.List;
using PetitesVictoires.UseCases.Users.GetLikeStats;
using PetitesVictoires.UseCases.Users.List;

namespace PetitesVictoires.Infrastructure.Configuration;

public static class QueryServicesConfiguration
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddQueryServicesConfiguration()
        {
            services
                .AddScoped<IListUsersQueryService, ListUsersQueryService>()
                .AddScoped<IGetPostQueryService, GetPostQueryService>()
                .AddScoped<IListPostsQueryService, ListPostsQueryService>()
                .AddScoped<IGetUserLikeStatsQueryService, GetUserLikeStatsQueryService>();

            return services;
        }
    }
}
