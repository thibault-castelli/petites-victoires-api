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
    private static readonly UserId AuthorId = UserId.From(1);
    private static readonly PostId SavedPostId = PostId.From(10);
    private static readonly PostContent Content = PostContent.From("hello");
    private static readonly Email AuthorEmail = Email.From("author@example.com");
    private static readonly UserName AuthorName = UserName.From("author");

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

    private static CreatePostCommand Command()
    {
        return new CreatePostCommand(Content, AuthorId);
    }

    private void ArrangeValid()
    {
        var savedPost = new Post(Content, AuthorId) { Id = SavedPostId };
        var author = new User(AuthorId, AuthorEmail, AuthorName);
        _postRepository.AddAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>()).Returns(savedPost);
        _userRepository.GetByIdAsync(AuthorId, CancellationToken.None).Returns(author);
    }

    [Test]
    public async Task Handle_WhenValid_ReturnsDtoMappedFromSavedPostAndAuthor()
    {
        ArrangeValid();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(SavedPostId);
        result.Value.Content.ShouldBe(Content);
        result.Value.UserEmailAddress.ShouldBe(AuthorEmail);
        result.Value.UserName.ShouldBe(AuthorName);
    }

    [Test]
    public async Task Handle_WhenValid_PersistsThePost()
    {
        ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        await _postRepository.Received(1).AddAsync(
            Arg.Is<Post>(p => p.Content == Content && p.UserId == AuthorId),
            Arg.Any<CancellationToken>());
    }
}
