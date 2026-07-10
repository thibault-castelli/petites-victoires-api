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
    private static readonly UserId LikerId = UserId.From(1);
    private static readonly PostId TargetPostId = PostId.From(2);

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IRepository<Like>>();
        _cache = Substitute.For<IDistributedCache>();
        _handler = new DeleteLikeHandler(_repository, _cache);
    }

    private IRepository<Like> _repository = null!;
    private IDistributedCache _cache = null!;
    private DeleteLikeHandler _handler = null!;

    private static DeleteLikeCommand Command()
    {
        return new DeleteLikeCommand(LikerId, TargetPostId);
    }

    private void ArrangeLikeMissing()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns((Like?)null);
    }

    private Like ArrangeExistingLike()
    {
        var like = new Like(LikerId, TargetPostId);
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns(like);
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

        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Like>(), Arg.Any<CancellationToken>());
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

        await _repository.Received(1).DeleteAsync(like, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenLikeExists_RemovesPostCache()
    {
        ArrangeExistingLike();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1)
            .RemoveAsync($"{Constants.PostCachePrefix}{TargetPostId.Value}", CancellationToken.None);
    }
}
