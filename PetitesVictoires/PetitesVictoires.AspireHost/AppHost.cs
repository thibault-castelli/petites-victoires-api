using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithRedisInsight();

var db = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin(pgAdmin => { pgAdmin.WithHostPort(5050); })
    .AddDatabase("petitesvictoires");

var mailpit = builder.AddMailPit("mailpit");

var api = builder.AddProject<PetitesVictoires_Api>("api")
    // .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(mailpit)
    .WaitFor(mailpit);

builder.Build().Run();
