using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Infrastructure.Data;
using Shouldly;

namespace PetitesVictoires.IntegrationTests.Data;

[TestFixture]
public class EfUnitOfWorkTests : IntegrationTestBase
{
    [Test]
    public async Task Commit_PersistsChanges()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        var unitOfWork = new EfUnitOfWork(DbContext);

        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            DbContext.Posts.Add(new Post(PostContent.From("committed"), UserId.From(1)));
            await DbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        await using var verify = DatabaseFixture.CreateContext();
        (await verify.Posts.CountAsync()).ShouldBe(1);
    }

    [Test]
    public async Task Rollback_DiscardsChanges()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        var unitOfWork = new EfUnitOfWork(DbContext);

        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            DbContext.Posts.Add(new Post(PostContent.From("rolled back"), UserId.From(1)));
            await DbContext.SaveChangesAsync();
            await transaction.RollbackAsync();
        }

        await using var verify = DatabaseFixture.CreateContext();
        (await verify.Posts.CountAsync()).ShouldBe(0);
    }
}
