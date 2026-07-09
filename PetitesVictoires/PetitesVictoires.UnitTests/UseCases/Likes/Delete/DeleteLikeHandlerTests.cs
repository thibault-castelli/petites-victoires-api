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
        _repository = Substitute.For<IRepository<Like>>();
        _cache = Substitute.For<IDistributedCache>();
        _handler = new DeleteLikeHandler(_repository, _cache);
    }

    private IRepository<Like> _repository = null!;
    private IDistributedCache _cache = null!;
    private DeleteLikeHandler _handler = null!;

    private static DeleteLikeCommand Command()
    {
        return new DeleteLikeCommand(UserId.From(1), PostId.From(2));
    }

    [Test]
    public async Task Handle_WhenLikeDoesNotExist_ReturnsNotFound()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns((Like?)null);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Like>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenLikeExists_DeletesIt()
    {
        var like = new Like(UserId.From(1), PostId.From(2));
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns(like);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _repository.Received(1).DeleteAsync(like, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenLikeDoesNotExist_DoesNotRemovesPostCache()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns((Like?)null);

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenLikeExists_RemovesPostCache()
    {
        var postId = PostId.From(2);
        var like = new Like(UserId.From(1), postId);
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns(like);

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1).RemoveAsync($"{Constants.PostCachePrefix}{postId.Value}", CancellationToken.None);
    }
}
