using NSubstitute;
using PetitesVictoires.UseCases;
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

    private static PagedResult<UserDto> EmptyPage(int page, int countPerPage)
    {
        return new PagedResult<UserDto>(new List<UserDto>(), page, countPerPage, 0, 0);
    }

    [Test]
    public async Task Handle_PassesRequestedPagingToTheQueryServiceAndReturnsItsResult()
    {
        var paged = EmptyPage(3, 20);
        _queryService.ListAsync(3, 20).Returns(paged);

        var result = await _handler.Handle(new ListUsersQuery(3, 20), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(paged);
        await _queryService.Received(1).ListAsync(3, 20);
    }

    [Test]
    public async Task Handle_WhenPagingIsNull_FallsBackToDefaults()
    {
        _queryService.ListAsync(1, Constants.DefaultPageSize).Returns(EmptyPage(1, Constants.DefaultPageSize));

        await _handler.Handle(new ListUsersQuery(null, null), CancellationToken.None);

        await _queryService.Received(1).ListAsync(1, Constants.DefaultPageSize);
    }
}
