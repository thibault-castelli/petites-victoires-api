using Microsoft.EntityFrameworkCore;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.LikeAggregate.Specifications;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.PostAggregate.Specifications;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.Core.UserAggregate.Specifications;
using PetitesVictoires.Infrastructure.Data;
using Shouldly;

namespace PetitesVictoires.IntegrationTests.Data;

[TestFixture]
public class EfRepositoryTests : IntegrationTestBase
{
    [Test]
    public async Task GetByIdAsync_WhenPostExists_ReturnsEntity()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);
        var repository = new EfRepository<Post>(DbContext);

        var result = await repository.GetByIdAsync(PostId.From(10), CancellationToken.None);

        result.ShouldNotBeNull();
        result.Content.Value.ShouldBe("content");
    }

    [Test]
    public async Task GetByIdAsync_WhenPostDoesNotExist_ReturnsNull()
    {
        var repository = new EfRepository<Post>(DbContext);

        var result = await repository.GetByIdAsync(PostId.From(999), CancellationToken.None);

        result.ShouldBeNull();
    }

    [Test]
    public async Task FirstOrDefaultAsync_PostByIdAndUser_WhenUserOwnsPost_ReturnsPost()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);
        var repository = new EfRepository<Post>(DbContext);

        var result = await repository.FirstOrDefaultAsync(
            new PostByIdAndUserSpecification(PostId.From(10), UserId.From(1)), CancellationToken.None);

        result.ShouldNotBeNull();
    }

    [Test]
    public async Task FirstOrDefaultAsync_PostByIdAndUser_WhenUserDoesNotOwnPost_ReturnsNull()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);
        var repository = new EfRepository<Post>(DbContext);

        var result = await repository.FirstOrDefaultAsync(
            new PostByIdAndUserSpecification(PostId.From(10), UserId.From(2)), CancellationToken.None);

        result.ShouldBeNull();
    }

    [Test]
    public async Task FirstOrDefaultAsync_PostById_WhenPostSoftDeleted_ReturnsNull()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);
        await SoftDeletePostAsync(10);
        var repository = new EfRepository<Post>(DbContext);

        var result = await repository.FirstOrDefaultAsync(
            new PostByIdSpecification(PostId.From(10)), CancellationToken.None);

        result.ShouldBeNull();
    }

    [Test]
    public async Task FirstOrDefaultAsync_UserById_ReturnsUser()
    {
        await SeedUserAsync(1, "alice@mail.com", "alice");
        var repository = new EfRepository<User>(DbContext);

        var result = await repository.FirstOrDefaultAsync(
            new UserByIdSpecification(UserId.From(1)), CancellationToken.None);

        result.ShouldNotBeNull();
        result.EmailAddress.Value.ShouldBe("alice@mail.com");
    }

    [Test]
    public async Task FirstOrDefaultAsync_LikeByUserAndPost_ReturnsMatchingLike()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);
        await SeedLikeAsync(1, 1, 10);
        var repository = new EfRepository<Like>(DbContext);

        var result = await repository.FirstOrDefaultAsync(
            new LikeByUserAndPostSpecification(UserId.From(1), PostId.From(10)), CancellationToken.None);

        result.ShouldNotBeNull();
    }

    [Test]
    public async Task FirstOrDefaultAsync_LikeByUserAndPost_WhenNoLike_ReturnsNull()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedPostAsync(10, "content", 1);
        var repository = new EfRepository<Like>(DbContext);

        var result = await repository.FirstOrDefaultAsync(
            new LikeByUserAndPostSpecification(UserId.From(1), PostId.From(10)), CancellationToken.None);

        result.ShouldBeNull();
    }

    [Test]
    public async Task ListAsync_LikeByPost_ReturnsAllLikesForThatPost()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        await SeedUserAsync(2, "b@mail.com", "bob");
        await SeedPostAsync(10, "content", 1);
        await SeedPostAsync(11, "other", 1);
        await SeedLikeAsync(1, 1, 10);
        await SeedLikeAsync(2, 2, 10);
        await SeedLikeAsync(3, 1, 11);
        var repository = new EfRepository<Like>(DbContext);

        var result = await repository.ListAsync(
            new LikeByPostSpecification(PostId.From(10)), CancellationToken.None);

        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task AddAsync_PersistsPost()
    {
        await SeedUserAsync(1, "a@mail.com", "alice");
        var repository = new EfRepository<Post>(DbContext);

        var saved = await repository.AddAsync(
            new Post(PostContent.From("new post"), UserId.From(1)), CancellationToken.None);

        await using var verify = DatabaseFixture.CreateContext();
        var persisted = await verify.Posts.FirstOrDefaultAsync(p => p.Id == saved.Id);
        persisted.ShouldNotBeNull();
    }
}
