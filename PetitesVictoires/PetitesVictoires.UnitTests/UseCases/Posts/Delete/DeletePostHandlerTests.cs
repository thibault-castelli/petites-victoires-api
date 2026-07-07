using Ardalis.Result;
using Ardalis.SharedKernel;
using NSubstitute;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Posts.Delete;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Posts.Delete;

[TestFixture]
public class DeletePostHandlerTests
{
    private IRepository<Post> _repository = null!;
    private DeletePostHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IRepository<Post>>();
        _handler = new DeletePostHandler(_repository);
    }

    private static Post ExistingPost(int ownerId)
    {
        return new Post(PostContent.From("content"), UserId.From(ownerId)) { Id = PostId.From(1) };
    }

    private static DeletePostCommand Command(int userId = 1)
    {
        return new DeletePostCommand(PostId.From(1), UserId.From(userId));
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_ReturnsNotFound()
    {
        _repository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns((Post?)null);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPostBelongsToAnotherUser_ReturnsForbidden()
    {
        _repository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns(ExistingPost(ownerId: 999));

        var result = await _handler.Handle(Command(userId: 1), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValid_SoftDeletesPostAndPersists()
    {
        var post = ExistingPost(ownerId: 1);
        _repository.GetByIdAsync(PostId.From(1), CancellationToken.None).Returns(post);

        var result = await _handler.Handle(Command(userId: 1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        post.DeletedAt.ShouldNotBeNull();
        await _repository.Received(1).UpdateAsync(post, CancellationToken.None);
    }
}
