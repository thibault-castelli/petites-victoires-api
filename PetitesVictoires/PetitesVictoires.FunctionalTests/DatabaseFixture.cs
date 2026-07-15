using PetitesVictoires.Infrastructure.Data;
using PetitesVictoires.TestSupport;

namespace PetitesVictoires.FunctionalTests;

[SetUpFixture]
public class DatabaseFixture
{
    private static readonly PostgresTestDatabase Database = new();

    public static string ConnectionString => Database.ConnectionString;

    /// <summary>One host for the whole assembly — booting the real app per fixture would be wasteful.</summary>
    public static CustomWebApplicationFactory Factory { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task GlobalSetUp()
    {
        await Database.StartAsync();

        // Program.cs reads these while registering services, which happens before any
        // WebApplicationFactory callback runs — so they have to be in the environment first.
        // "Testing" (not Development) also stops the app auto-migrating and seeding over our data.
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("ConnectionStrings__petitesvictoires", Database.ConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__cache", "localhost:6379,abortConnect=false");

        Factory = new CustomWebApplicationFactory();
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        await Factory.DisposeAsync();
        await Database.StopAsync();
    }

    public static Task ResetAsync() => Database.ResetAsync();

    public static PetitesVictoiresDbContext CreateContext() => Database.CreateContext();
}
