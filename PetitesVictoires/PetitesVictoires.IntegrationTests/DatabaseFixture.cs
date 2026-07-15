using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.TestSupport;

namespace PetitesVictoires.IntegrationTests;

[SetUpFixture]
public class DatabaseFixture
{
    private static readonly PostgresTestDatabase Database = new();

    public static string ConnectionString => Database.ConnectionString;

    [OneTimeSetUp]
    public Task GlobalSetUp() => Database.StartAsync();

    [OneTimeTearDown]
    public Task GlobalTearDown() => Database.StopAsync();

    public static Task ResetAsync() => Database.ResetAsync();

    public static PetitesVictoiresDbContext CreateContext(bool withInterceptors = false) =>
        Database.CreateContext(withInterceptors);
}
