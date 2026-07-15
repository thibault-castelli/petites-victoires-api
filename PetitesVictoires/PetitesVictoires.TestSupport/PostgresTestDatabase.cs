using Microsoft.EntityFrameworkCore;
using Npgsql;
using PetitesVictoires.Infrastructure.Data;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace PetitesVictoires.TestSupport;

/// <summary>
///     Owns one throwaway Postgres container for a test assembly: starts it, applies the real EF
///     migrations, and snapshots the empty schema so each test can reset back to it.
///     [SetUpFixture] is per-assembly, so each test project drives its own instance.
/// </summary>
public sealed class PostgresTestDatabase
{
    private PostgreSqlContainer _container = null!;
    private Respawner _respawner = null!;

    public string ConnectionString { get; private set; } = null!;

    public async Task StartAsync()
    {
        _container = new PostgreSqlBuilder("postgres:17")
            .Build();
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();

        // Build the schema once, from the real EF migrations.
        await using var dbContext = CreateContext();
        await dbContext.Database.MigrateAsync();

        // Snapshot the empty schema so every test can reset back to it.
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = [new Table("__EFMigrationsHistory")]
        });
    }

    public async Task StopAsync() => await _container.DisposeAsync();

    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public PetitesVictoiresDbContext CreateContext(bool withInterceptors = false)
    {
        var builder = new DbContextOptionsBuilder<PetitesVictoiresDbContext>()
            .UseNpgsql(ConnectionString);

        // Reads go through a bare context; interceptor and repository-write tests need the real
        // SaveChanges pipeline (audit stamping + hard-delete -> soft-delete).
        if (withInterceptors)
            builder.AddInterceptors(new AuditableInterceptor());

        return new PetitesVictoiresDbContext(builder.Options);
    }
}
