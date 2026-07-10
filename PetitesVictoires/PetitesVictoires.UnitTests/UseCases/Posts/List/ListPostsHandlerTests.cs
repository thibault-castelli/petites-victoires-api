using NSubstitute;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Posts;
using PetitesVictoires.UseCases.Posts.List;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Posts.List;

[TestFixture]
public class ListPostsHandlerTests
{
    private const int RequestedPage = 2;
    private const int RequestedCountPerPage = 5;

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
    public async Task Handle_ReturnsTheResultFromTheQueryService()
    {
        var paged = EmptyPage(RequestedPage, RequestedCountPerPage);
        _queryService.ListAsync(RequestedPage, RequestedCountPerPage).Returns(paged);

        var result = await _handler.Handle(new ListPostsQuery(RequestedPage, RequestedCountPerPage),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(paged);
    }

    [Test]
    public async Task Handle_PassesRequestedPagingToTheQueryService()
    {
        _queryService.ListAsync(RequestedPage, RequestedCountPerPage)
            .Returns(EmptyPage(RequestedPage, RequestedCountPerPage));

        await _handler.Handle(new ListPostsQuery(RequestedPage, RequestedCountPerPage), CancellationToken.None);

        await _queryService.Received(1).ListAsync(RequestedPage, RequestedCountPerPage);
    }

    [Test]
    public async Task Handle_WhenPagingIsNull_FallsBackToDefaults()
    {
        _queryService.ListAsync(1, Constants.DefaultPageSize).Returns(EmptyPage(1, Constants.DefaultPageSize));

        await _handler.Handle(new ListPostsQuery(null, null), CancellationToken.None);

        await _queryService.Received(1).ListAsync(1, Constants.DefaultPageSize);
    }
}
