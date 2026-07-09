using Ardalis.Result;
using Ardalis.SharedKernel;
using Ardalis.Specification;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Likes.Create;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Likes.Create;

[TestFixture]
public class CreateLikeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _likeRepository = Substitute.For<IRepository<Like>>();
        _postRepository = Substitute.For<IReadRepository<Post>>();
        _cache = Substitute.For<IDistributedCache>();
        _handler = new CreateLikeHandler(_likeRepository, _postRepository, _cache);
    }

    private IRepository<Like> _likeRepository = null!;
    private IReadRepository<Post> _postRepository = null!;
    private IDistributedCache _cache = null!;
    private CreateLikeHandler _handler = null!;

    private static CreateLikeCommand Command()
    {
        return new CreateLikeCommand(UserId.From(1), PostId.From(2));
    }

    private static Post ExistingPost()
    {
        return new Post(PostContent.From("content"), UserId.From(9)) { Id = PostId.From(2) };
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_ReturnsNotFound()
    {
        _postRepository.GetByIdAsync(PostId.From(2), CancellationToken.None).Returns((Post?)null);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        await _likeRepository.DidNotReceive().AddAsync(Arg.Any<Like>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenLikeAlreadyExists_ReturnsConflict()
    {
        var postId = PostId.From(2);
        _postRepository.GetByIdAsync(postId, CancellationToken.None).Returns(ExistingPost());
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns(new Like(UserId.From(1), postId));

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _likeRepository.DidNotReceive().AddAsync(Arg.Any<Like>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValid_AddsTheLike()
    {
        var postId = PostId.From(2);
        _postRepository.GetByIdAsync(postId, CancellationToken.None).Returns(ExistingPost());
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns((Like?)null);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _likeRepository.Received(1).AddAsync(
            Arg.Is<Like>(l => l.UserId == UserId.From(1) && l.PostId == postId),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValid_RemovesPostCache()
    {
        var postId = PostId.From(2);
        _postRepository.GetByIdAsync(postId, CancellationToken.None).Returns(ExistingPost());
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns((Like?)null);

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1).RemoveAsync($"{Constants.PostCachePrefix}{postId.Value}", CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_DoesNotRemovesPostCache()
    {
        var postId = PostId.From(2);
        _postRepository.GetByIdAsync(postId, CancellationToken.None).Returns((Post?)null);

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync($"{Constants.PostCachePrefix}{postId.Value}", CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenLikeAlreadyExists_DoesNotRemovesPostCache()
    {
        var postId = PostId.From(2);
        _postRepository.GetByIdAsync(postId, CancellationToken.None).Returns(ExistingPost());
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns(new Like(UserId.From(1), postId));

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync($"{Constants.PostCachePrefix}{postId.Value}", CancellationToken.None);
    }
}
