using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.Infrastructure.Data;

public class PetitesVictoiresDbContext(DbContextOptions<PetitesVictoiresDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
