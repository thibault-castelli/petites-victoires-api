using Ardalis.Result;
using Ardalis.SharedKernel;
using Ardalis.Specification;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Likes.Delete;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Likes.Delete;

[TestFixture]
public class DeleteLikeHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _likeRepository = Substitute.For<IRepository<Like>>();
        _postRepository = Substitute.For<IReadRepository<Post>>();
        _cache = Substitute.For<IDistributedCache>();
        _handler = new DeleteLikeHandler(_likeRepository, _postRepository, _cache);
    }

    private static readonly UserId LikerId = UserId.From(1);
    private static readonly PostId TargetPostId = PostId.From(2);
    private static readonly UserId PostAuthorId = UserId.From(9);

    private IRepository<Like> _likeRepository = null!;
    private IReadRepository<Post> _postRepository = null!;
    private IDistributedCache _cache = null!;
    private DeleteLikeHandler _handler = null!;

    private static DeleteLikeCommand Command()
    {
        return new DeleteLikeCommand(LikerId, TargetPostId);
    }

    private void ArrangeLikeMissing()
    {
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns((Like?)null);
    }

    private static Post ExistingPost()
    {
        return new Post(PostContent.From("content"), PostAuthorId) { Id = TargetPostId };
    }

    private Like ArrangeExistingLike()
    {
        var like = new Like(LikerId, TargetPostId);
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns(like);
        _postRepository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns(ExistingPost());
        return like;
    }

    [Test]
    public async Task Handle_WhenLikeDoesNotExist_ReturnsNotFound()
    {
        ArrangeLikeMissing();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenLikeDoesNotExist_DoesNotDeleteLike()
    {
        ArrangeLikeMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _likeRepository.DidNotReceive().DeleteAsync(Arg.Any<Like>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenLikeDoesNotExist_DoesNotRemovePostCache()
    {
        ArrangeLikeMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenLikeExists_ReturnsSuccess()
    {
        ArrangeExistingLike();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Handle_WhenLikeExists_DeletesIt()
    {
        var like = ArrangeExistingLike();

        await _handler.Handle(Command(), CancellationToken.None);

        await _likeRepository.Received(1).DeleteAsync(like, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenLikeExists_RemovesPostCache()
    {
        ArrangeExistingLike();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1)
            .RemoveAsync($"{Constants.PostCachePrefix}{TargetPostId.Value}", CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenLikeExists_RemovesLikerLikeStatsCache()
    {
        ArrangeExistingLike();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1)
            .RemoveAsync($"{Constants.UserLikeStatsCachePrefix}{LikerId}", CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenLikeExists_RemovesPostAuthorLikeStatsCache()
    {
        ArrangeExistingLike();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1)
            .RemoveAsync($"{Constants.UserLikeStatsCachePrefix}{PostAuthorId}", CancellationToken.None);
    }
}
