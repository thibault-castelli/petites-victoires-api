using Ardalis.Result;
using Ardalis.SharedKernel;
using Ardalis.Specification;
using NSubstitute;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Likes.Delete;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Likes.Delete;

[TestFixture]
public class DeleteLikeHandlerTests
{
    private IRepository<Like> _repository = null!;
    private DeleteLikeHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IRepository<Like>>();
        _handler = new DeleteLikeHandler(_repository);
    }

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
}
