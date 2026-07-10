using Ardalis.Result;
using Ardalis.SharedKernel;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts.Update;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Posts.Update;

[TestFixture]
public class UpdatePostHandlerTests
{
    private static readonly PostId TargetPostId = PostId.From(1);
    private static readonly UserId RequesterId = UserId.From(1);
    private static readonly UserId OtherUserId = UserId.From(999);
    private static readonly PostContent UpdatedContent = PostContent.From("updated");
    private static readonly Email OwnerEmail = Email.From("owner@example.com");
    private static readonly UserName OwnerName = UserName.From("owner");

    [SetUp]
    public void SetUp()
    {
        _postRepository = Substitute.For<IRepository<Post>>();
        _userRepository = Substitute.For<IReadRepository<User>>();
        _likeRepository = Substitute.For<IReadRepository<Like>>();
        _cache = Substitute.For<IDistributedCache>();
        _handler = new UpdatePostHandler(_postRepository, _userRepository, _likeRepository, _cache);
    }

    private IRepository<Post> _postRepository = null!;
    private IReadRepository<User> _userRepository = null!;
    private IReadRepository<Like> _likeRepository = null!;
    private IDistributedCache _cache = null!;
    private UpdatePostHandler _handler = null!;

    private static Post PostOwnedBy(UserId ownerId)
    {
        return new Post(PostContent.From("original"), ownerId) { Id = TargetPostId };
    }

    private static UpdatePostCommand Command()
    {
        return new UpdatePostCommand(TargetPostId, UpdatedContent, RequesterId);
    }

    private void ArrangePostMissing()
    {
        _postRepository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns((Post?)null);
    }

    private void ArrangePostOwnedByAnotherUser()
    {
        _postRepository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns(PostOwnedBy(OtherUserId));
    }

    private void ArrangeAuthorMissing()
    {
        _postRepository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns(PostOwnedBy(RequesterId));
        _userRepository.GetByIdAsync(RequesterId, CancellationToken.None).Returns((User?)null);
    }

    private Post ArrangeValid()
    {
        var post = PostOwnedBy(RequesterId);
        var user = new User(RequesterId, OwnerEmail, OwnerName);
        _postRepository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns(post);
        _userRepository.GetByIdAsync(RequesterId, CancellationToken.None).Returns(user);
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

        await _postRepository.DidNotReceive().UpdateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
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

        await _postRepository.DidNotReceive().UpdateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPostBelongsToAnotherUser_DoesNotRemoveFromCache()
    {
        ArrangePostOwnedByAnotherUser();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        ArrangeAuthorMissing();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_DoesNotPersist()
    {
        ArrangeAuthorMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _postRepository.DidNotReceive().UpdateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_DoesNotRemoveFromCache()
    {
        ArrangeAuthorMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValid_ReturnsMappedDto()
    {
        ArrangeValid();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(UpdatedContent);
        result.Value.UserEmailAddress.ShouldBe(OwnerEmail);
    }

    [Test]
    public async Task Handle_WhenValid_UpdatesTheContent()
    {
        var post = ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        post.Content.ShouldBe(UpdatedContent);
    }

    [Test]
    public async Task Handle_WhenValid_PersistsThePost()
    {
        var post = ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        await _postRepository.Received(1).UpdateAsync(post, CancellationToken.None);
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
