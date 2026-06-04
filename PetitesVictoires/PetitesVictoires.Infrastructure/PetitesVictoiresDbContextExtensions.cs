using Microsoft.Extensions.Hosting;

namespace PetitesVictoires.Infrastructure;

public static class PetitesVictoiresDbContextExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddPetitesVictoiresDbContext()
        {
            builder.AddNpgsqlDbContext<PetitesVictoiresDbContext>("petitesvictoires");
        }
    }
}
