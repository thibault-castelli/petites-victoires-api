using Ardalis.Result;
using Ardalis.SharedKernel;
using Ardalis.Specification;
using NSubstitute;
using PetitesVictoires.Core.LikeAggregate;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Likes.Create;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Likes.Create;

[TestFixture]
public class CreateLikeHandlerTests
{
    private IRepository<Like> _likeRepository = null!;
    private IReadRepository<Post> _postRepository = null!;
    private CreateLikeHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _likeRepository = Substitute.For<IRepository<Like>>();
        _postRepository = Substitute.For<IReadRepository<Post>>();
        _handler = new CreateLikeHandler(_likeRepository, _postRepository);
    }

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
        _postRepository.GetByIdAsync(PostId.From(2), CancellationToken.None).Returns(ExistingPost());
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns(new Like(UserId.From(1), PostId.From(2)));

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _likeRepository.DidNotReceive().AddAsync(Arg.Any<Like>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValid_AddsTheLike()
    {
        _postRepository.GetByIdAsync(PostId.From(2), CancellationToken.None).Returns(ExistingPost());
        _likeRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Like>>(), Arg.Any<CancellationToken>())
            .Returns((Like?)null);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _likeRepository.Received(1).AddAsync(
            Arg.Is<Like>(l => l.UserId == UserId.From(1) && l.PostId == PostId.From(2)),
            Arg.Any<CancellationToken>());
    }
}
