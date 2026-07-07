using Ardalis.SharedKernel;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Posts.Create;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Posts.Create;

[TestFixture]
public class CreatePostHandlerTests
{
    private IRepository<Post> _postRepository = null!;
    private IReadRepository<User> _userRepository = null!;
    private CreatePostHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _postRepository = Substitute.For<IRepository<Post>>();
        _userRepository = Substitute.For<IReadRepository<User>>();
        _handler = new CreatePostHandler(_postRepository, _userRepository);
    }

    [Test]
    public async Task Handle_PersistsPostAndReturnsDtoMappedFromSavedPostAndAuthor()
    {
        var savedPost = new Post(PostContent.From("hello"), UserId.From(1)) { Id = PostId.From(10) };
        var author = new User(UserId.From(1), Email.From("author@example.com"), UserName.From("author"));
        _postRepository.AddAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>()).Returns(savedPost);
        _userRepository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns(author);

        var command = new CreatePostCommand(PostContent.From("hello"), UserId.From(1));
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(PostId.From(10));
        result.Value.Content.ShouldBe(PostContent.From("hello"));
        result.Value.UserEmailAddress.ShouldBe(Email.From("author@example.com"));
        result.Value.UserName.ShouldBe(UserName.From("author"));
        await _postRepository.Received(1).AddAsync(
            Arg.Is<Post>(p => p.Content == PostContent.From("hello") && p.UserId == UserId.From(1)),
            Arg.Any<CancellationToken>());
    }
}
