using Ardalis.Result;
using Ardalis.SharedKernel;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts.Update;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Posts.Update;

[TestFixture]
public class UpdatePostHandlerTests
{
    private IRepository<Post> _postRepository = null!;
    private IReadRepository<User> _userRepository = null!;
    private IDistributedCache _cache = null!;
    private UpdatePostHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _postRepository = Substitute.For<IRepository<Post>>();
        _userRepository = Substitute.For<IReadRepository<User>>();
        _cache = Substitute.For<IDistributedCache>();
        _handler = new UpdatePostHandler(_postRepository, _userRepository, _cache);
    }

    private static Post ExistingPost(int ownerId)
    {
        return new Post(PostContent.From("original"), UserId.From(ownerId)) { Id = PostId.From(1) };
    }

    private static UpdatePostCommand Command(int userId = 1, string content = "updated")
    {
        return new UpdatePostCommand(PostId.From(1), PostContent.From(content), UserId.From(userId));
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_ReturnsNotFound()
    {
        _postRepository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns((Post?)null);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        await _postRepository.DidNotReceive().UpdateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPostBelongsToAnotherUser_ReturnsForbidden()
    {
        _postRepository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns(ExistingPost(ownerId: 999));

        var result = await _handler.Handle(Command(userId: 1), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _postRepository.DidNotReceive().UpdateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        _postRepository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns(ExistingPost(ownerId: 1));
        _userRepository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns((User?)null);

        var result = await _handler.Handle(Command(userId: 1), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenValid_UpdatesContentAndReturnsMappedDto()
    {
        var post = ExistingPost(ownerId: 1);
        var user = new User(UserId.From(1), Email.From("owner@example.com"), UserName.From("owner"));
        _postRepository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns(post);
        _userRepository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns(user);

        var result = await _handler.Handle(Command(userId: 1, content: "updated"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(PostContent.From("updated"));
        result.Value.UserEmailAddress.ShouldBe(Email.From("owner@example.com"));
        post.Content.ShouldBe(PostContent.From("updated"));
        await _postRepository.Received(1).UpdateAsync(post, CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenValid_RemovesPostFromCache()
    {
        var post = ExistingPost(ownerId: 1);
        var user = new User(UserId.From(1), Email.From("owner@example.com"), UserName.From("owner"));
        _postRepository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns(post);
        _userRepository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns(user);

        await _handler.Handle(Command(userId: 1), CancellationToken.None);

        await _cache.Received(1).RemoveAsync($"{Constants.PostCachePrefix}{post.Id.Value}", CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_DoesNotRemoveFromCache()
    {
        _postRepository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns((Post?)null);

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPostBelongsToAnotherUser_DoesNotRemoveFromCache()
    {
        _postRepository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns(ExistingPost(ownerId: 999));

        await _handler.Handle(Command(userId: 1), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_DoesNotRemoveFromCache()
    {
        _postRepository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns(ExistingPost(ownerId: 1));
        _userRepository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns((User?)null);

        await _handler.Handle(Command(userId: 1), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
