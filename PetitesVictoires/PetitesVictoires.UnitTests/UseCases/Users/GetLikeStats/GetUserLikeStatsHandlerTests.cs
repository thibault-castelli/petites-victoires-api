using Ardalis.Result;
using Ardalis.SharedKernel;
using Ardalis.Specification;
using NSubstitute;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users;
using PetitesVictoires.UseCases.Users.GetLikeStats;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.GetLikeStats;

[TestFixture]
public class GetUserLikeStatsHandlerTests
{
    private static readonly UserId TargetUserId = UserId.From(1);

    private IReadRepository<User> _repository = null!;
    private IGetUserLikeStatsQueryService _queryService = null!;
    private GetUserLikeStatsHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IReadRepository<User>>();
        _queryService = Substitute.For<IGetUserLikeStatsQueryService>();
        _handler = new GetUserLikeStatsHandler(_repository, _queryService);
    }

    private static GetUserLikeStatsQuery Query()
    {
        return new GetUserLikeStatsQuery(TargetUserId);
    }

    private void ArrangeUserExists(bool exists)
    {
        _repository.AnyAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>()).Returns(exists);
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        ArrangeUserExists(false);

        var result = await _handler.Handle(Query(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_DoesNotQueryStats()
    {
        ArrangeUserExists(false);

        await _handler.Handle(Query(), CancellationToken.None);

        await _queryService.DidNotReceiveWithAnyArgs().GetUserLikeStatsAsync(TargetUserId);
    }

    [Test]
    public async Task Handle_WhenUserExists_ReturnsStatsFromQueryService()
    {
        ArrangeUserExists(true);
        var stats = new UserLikeStatsDto(3, 7);
        _queryService.GetUserLikeStatsAsync(TargetUserId).Returns(stats);

        var result = await _handler.Handle(Query(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(stats);
    }
}
