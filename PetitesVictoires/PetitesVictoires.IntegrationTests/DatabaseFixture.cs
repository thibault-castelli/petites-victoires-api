using Microsoft.EntityFrameworkCore;
using Npgsql;
using PetitesVictoires.Infrastructure.Data;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace PetitesVictoires.IntegrationTests;

[SetUpFixture]
public class DatabaseFixture
{
    private static PostgreSqlContainer _container = null!;
    private static Respawner _respawner = null!;

    public static string ConnectionString { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task GlobalSetUp()
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

    [OneTimeTearDown]
    public async Task GlobalTearDown() => await _container.DisposeAsync();

    public static async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public static PetitesVictoiresDbContext CreateContext(bool withInterceptors = false)
    {
        var builder = new DbContextOptionsBuilder<PetitesVictoiresDbContext>()
            .UseNpgsql(ConnectionString);

        // The query-service tests read through a bare context; interceptor and repository-write
        // tests need the real SaveChanges pipeline (audit stamping + hard-delete -> soft-delete).
        if (withInterceptors)
            builder.AddInterceptors(new AuditableInterceptor());

        return new PetitesVictoiresDbContext(builder.Options);
    }
}
