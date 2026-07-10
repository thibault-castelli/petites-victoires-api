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
    private static readonly UserId LikerId = UserId.From(1);
    private static readonly PostId TargetPostId = PostId.From(2);
    private static readonly UserId PostAuthorId = UserId.From(9);

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
        return new CreateLikeCommand(LikerId, TargetPostId);
    }

    private static Post ExistingPost()
    {
        return new Post(PostContent.From("content"), PostAuthorId) { Id = TargetPostId };
    }

    private void ArrangePostMissing()
    {
        _postRepository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns((Post?)null);
    }

    private void ArrangeExistingLike()
    {
        _postRepository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns(ExistingPost());
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns(new Like(LikerId, TargetPostId));
    }

    private void ArrangeValid()
    {
        _postRepository.GetByIdAsync(TargetPostId, CancellationToken.None).Returns(ExistingPost());
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns((Like?)null);
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_ReturnsNotFound()
    {
        ArrangePostMissing();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_DoesNotAddLike()
    {
        ArrangePostMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _likeRepository.DidNotReceive().AddAsync(Arg.Any<Like>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_DoesNotRemovePostCache()
    {
        ArrangePostMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenLikeAlreadyExists_ReturnsConflict()
    {
        ArrangeExistingLike();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
    }

    [Test]
    public async Task Handle_WhenLikeAlreadyExists_DoesNotAddLike()
    {
        ArrangeExistingLike();

        await _handler.Handle(Command(), CancellationToken.None);

        await _likeRepository.DidNotReceive().AddAsync(Arg.Any<Like>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenLikeAlreadyExists_DoesNotRemovePostCache()
    {
        ArrangeExistingLike();

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
    public async Task Handle_WhenValid_AddsTheLike()
    {
        ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        await _likeRepository.Received(1).AddAsync(
            Arg.Is<Like>(l => l.UserId == LikerId && l.PostId == TargetPostId),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValid_RemovesPostCache()
    {
        ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1)
            .RemoveAsync($"{Constants.PostCachePrefix}{TargetPostId.Value}", CancellationToken.None);
    }
}
