using NSubstitute;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Common;
using PetitesVictoires.UseCases.Users;
using PetitesVictoires.UseCases.Users.List;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.List;

[TestFixture]
public class ListUsersHandlerTests
{
    private IListUsersQueryService _queryService = null!;
    private ListUsersHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _queryService = Substitute.For<IListUsersQueryService>();
        _handler = new ListUsersHandler(_queryService);
    }

    private static ListUsersQuery Query()
    {
        return new ListUsersQuery(
            new ListQueryParams(3, 20),
            new ListUsersCriteria("example"));
    }

    private static PagedResult<UserDto> EmptyPage()
    {
        return new PagedResult<UserDto>(new List<UserDto>(), 3, 20, 0, 0);
    }

    [Test]
    public async Task Handle_ReturnsTheResultFromTheQueryService()
    {
        var paged = EmptyPage();
        _queryService
            .ListAsync(Arg.Any<ListQueryParams>(), Arg.Any<ListUsersCriteria>(), Arg.Any<CancellationToken>())
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
            .ListAsync(Arg.Any<ListQueryParams>(), Arg.Any<ListUsersCriteria>(), Arg.Any<CancellationToken>())
            .Returns(EmptyPage());

        await _handler.Handle(query, cts.Token);

        await _queryService.Received(1)
            .ListAsync(query.ListQueryParams, query.ListUsersCriteria, cts.Token);
    }
}
