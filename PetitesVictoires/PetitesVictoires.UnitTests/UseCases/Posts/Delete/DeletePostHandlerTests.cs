using Ardalis.Result;
using Ardalis.SharedKernel;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts.Delete;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Posts.Delete;

[TestFixture]
public class DeletePostHandlerTests
{
    private static readonly PostId TargetPostId = PostId.From(1);
    private static readonly UserId RequesterId = UserId.From(1);
    private static readonly UserId OtherUserId = UserId.From(999);

    private IRepository<Post> _repository = null!;
    private IDistributedCache _cache = null!;
    private DeletePostHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IRepository<Post>>();
        _cache = Substitute.For<IDistributedCache>();
        _handler = new DeletePostHandler(_repository, _cache);
    }

    private static Post PostOwnedBy(UserId ownerId)
    {
        return new Post(PostContent.From("content"), ownerId) { Id = TargetPostId };
    }

    private static DeletePostCommand Command()
    {
        return new DeletePostCommand(TargetPostId, RequesterId);
    }

    private void ArrangePostMissing()
    {
        _repository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns((Post?)null);
    }

    private void ArrangePostOwnedByAnotherUser()
    {
        _repository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns(PostOwnedBy(OtherUserId));
    }

    private Post ArrangeValid()
    {
        var post = PostOwnedBy(RequesterId);
        _repository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns(post);
        return post;
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_ReturnsNotFound()
    {
        ArrangePostMissing();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_DoesNotPersist()
    {
        ArrangePostMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_DoesNotRemoveFromCache()
    {
        ArrangePostMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPostBelongsToAnotherUser_ReturnsForbidden()
    {
        ArrangePostOwnedByAnotherUser();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
    }

    [Test]
    public async Task Handle_WhenPostBelongsToAnotherUser_DoesNotPersist()
    {
        ArrangePostOwnedByAnotherUser();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPostBelongsToAnotherUser_DoesNotRemoveFromCache()
    {
        ArrangePostOwnedByAnotherUser();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValid_ReturnsSuccess()
    {
        ArrangeValid();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Handle_WhenValid_SoftDeletesThePost()
    {
        var post = ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        post.DeletedAt.ShouldNotBeNull();
    }

    [Test]
    public async Task Handle_WhenValid_PersistsThePost()
    {
        var post = ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.Received(1).UpdateAsync(post, CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenValid_RemovesPostFromCache()
    {
        ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1)
            .RemoveAsync($"{Constants.PostCachePrefix}{TargetPostId.Value}", CancellationToken.None);
    }
}
