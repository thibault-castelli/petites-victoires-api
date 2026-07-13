using NSubstitute;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Common;
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

    private static ListPostsQuery Query()
    {
        return new ListPostsQuery(
            new ListQueryParams(2, 5),
            new ListPostsCriteria(PostSortBy.LikesCount, UserId.From(3), UserId.From(7)));
    }

    private static PagedResult<PostDto> EmptyPage()
    {
        return new PagedResult<PostDto>(new List<PostDto>(), 2, 5, 0, 0);
    }

    [Test]
    public async Task Handle_ReturnsTheResultFromTheQueryService()
    {
        var paged = EmptyPage();
        _queryService
            .ListAsync(Arg.Any<ListQueryParams>(), Arg.Any<ListPostsCriteria>(), Arg.Any<CancellationToken>())
            .Returns(paged);

        var result = await _handler.Handle(Query(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(paged);
    }

    [Test]
    public async Task Handle_ForwardsParamsCriteriaAndTokenToTheQueryService()
    {
        using var cts = new CancellationTokenSource();
        var query = Query();
        _queryService
            .ListAsync(Arg.Any<ListQueryParams>(), Arg.Any<ListPostsCriteria>(), Arg.Any<CancellationToken>())
            .Returns(EmptyPage());

        await _handler.Handle(query, cts.Token);

        await _queryService.Received(1)
            .ListAsync(query.ListQueryParams, query.ListPostsCriteria, cts.Token);
    }
}
