using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using Shouldly;

namespace PetitesVictoires.UnitTests.Core.PostAggregate;

[TestFixture]
public class PostTests
{
    private static readonly UserId AuthorId = UserId.From(1);

    private static Post CreatePost(string content = "Initial content")
    {
        return new Post(PostContent.From(content), AuthorId);
    }

    [Test]
    public void Constructor_SetsContentAndAuthor()
    {
        var content = PostContent.From("My first victory");

        var post = new Post(content, AuthorId);

        post.Content.ShouldBe(content);
        post.UserId.ShouldBe(AuthorId);
    }

    [Test]
    public void Constructor_StampsCreatedAtWithUtcNow()
    {
        var before = DateTime.UtcNow;

        var post = CreatePost();

        post.CreatedAt.ShouldBeInRange(before, DateTime.UtcNow);
    }

    [Test]
    public void UpdateContent_WithDifferentContent_ReplacesTheContent()
    {
        var post = CreatePost("Old content");
        var newContent = PostContent.From("New content");

        post.UpdateContent(newContent);

        post.Content.ShouldBe(newContent);
    }

    [Test]
    public void UpdateContent_ReturnsTheSameInstance_ForFluentChaining()
    {
        var post = CreatePost("Old content");

        var result = post.UpdateContent(PostContent.From("New content"));

        result.ShouldBeSameAs(post);
    }

    [Test]
    public void UpdateContent_WithIdenticalContent_LeavesContentUnchanged()
    {
        var post = CreatePost("Same content");

        post.UpdateContent(PostContent.From("Same content"));

        post.Content.ShouldBe(PostContent.From("Same content"));
    }

    // Note: CreatedAt stays here (not in BaseEntityTests) because BaseEntity only declares the
    // property — Post's own constructor is what stamps it. MarkUpdated/MarkSoftDeleted, being
    // shared base behavior, are tested once in BaseEntityTests instead.
}
