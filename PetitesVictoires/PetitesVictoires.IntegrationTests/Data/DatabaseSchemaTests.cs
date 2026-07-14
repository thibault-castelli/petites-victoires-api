using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Infrastructure.Data;
using Shouldly;

namespace PetitesVictoires.IntegrationTests.Data;

[TestFixture]
public class DatabaseSchemaTests : IntegrationTestBase
{
    [Test]
    public async Task Likes_SameUserAndPostTwice_ViolatesUniqueIndex()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);
        await SeedLikeAsync(1, 1, 10);
        var repository = new EfRepository<Like>(DbContext);

        await Should.ThrowAsync<DbUpdateException>(async () =>
            await repository.AddAsync(new Like(UserId.From(1), PostId.From(10)), CancellationToken.None));
    }

    [Test]
    public void Model_HasNoPendingChangesMissingFromMigrations()
    {
        DbContext.Database.HasPendingModelChanges().ShouldBeFalse();
    }
}
