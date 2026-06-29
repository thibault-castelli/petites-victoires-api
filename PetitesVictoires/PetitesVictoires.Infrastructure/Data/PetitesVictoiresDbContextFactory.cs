using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PetitesVictoires.Infrastructure.Data;

/// <summary>
/// Design-time factory used by the EF Core tools (dotnet ef migrations / database update).
/// At design time Aspire is not orchestrating the app, so the runtime connection string
/// (injected via .WithReference(db)) is unavailable. The connection string below only needs
/// to be parseable by Npgsql to scaffold migrations - no database connection is made for
/// 'migrations add'.
/// </summary>
public class PetitesVictoiresDbContextFactory : IDesignTimeDbContextFactory<PetitesVictoiresDbContext>
{
    public PetitesVictoiresDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PetitesVictoiresDbContext>()
            .UseNpgsql("Host=localhost;Database=petitesvictoires;Username=postgres;Password=postgres")
            .Options;

        return new PetitesVictoiresDbContext(options);
    }
}
