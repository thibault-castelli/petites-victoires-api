using Ardalis.Result;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.PostAggregate;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Posts;
using PetitesVictoires.UseCases.Posts.Get;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Posts.Get;

[TestFixture]
public class GetPostHandlerTests
{
    private IGetPostQueryService _queryService = null!;
    private GetPostHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _queryService = Substitute.For<IGetPostQueryService>();
        _handler = new GetPostHandler(_queryService);
    }

    [Test]
    public async Task Handle_WhenPostDoesNotExist_ReturnsNotFound()
    {
        _queryService.GetPostAsync(PostId.From(1)).Returns((PostDto?)null);

        var result = await _handler.Handle(new GetPostQuery(PostId.From(1)), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenPostExists_ReturnsItFromTheQueryService()
    {
        var dto = new PostDto(PostId.From(1), PostContent.From("content"), UserId.From(1),
            Email.From("user@example.com"), UserName.From("user"), DateTime.UtcNow);
        _queryService.GetPostAsync(PostId.From(1)).Returns(dto);

        var result = await _handler.Handle(new GetPostQuery(PostId.From(1)), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(dto);
    }
}
