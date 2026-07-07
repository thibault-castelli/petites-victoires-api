using NSubstitute;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts;
using PetitesVictoires.UseCases.Posts.List;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Posts.List;

[TestFixture]
public class ListPostsHandlerTests
{
    private IListPostsQueryService _queryService = null!;
    private ListPostsHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _queryService = Substitute.For<IListPostsQueryService>();
        _handler = new ListPostsHandler(_queryService);
    }

    private static PagedResult<PostDto> EmptyPage(int page, int countPerPage)
    {
        return new PagedResult<PostDto>(new List<PostDto>(), page, countPerPage, 0, 0);
    }

    [Test]
    public async Task Handle_PassesRequestedPagingToTheQueryServiceAndReturnsItsResult()
    {
        var paged = EmptyPage(2, 5);
        _queryService.ListAsync(2, 5).Returns(paged);

        var result = await _handler.Handle(new ListPostsQuery(2, 5), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(paged);
        await _queryService.Received(1).ListAsync(2, 5);
    }

    [Test]
    public async Task Handle_WhenPagingIsNull_FallsBackToDefaults()
    {
        _queryService.ListAsync(1, Constants.DefaultPageSize).Returns(EmptyPage(1, Constants.DefaultPageSize));

        await _handler.Handle(new ListPostsQuery(null, null), CancellationToken.None);

        await _queryService.Received(1).ListAsync(1, Constants.DefaultPageSize);
    }
}
