using Ardalis.Result;
using Ardalis.SharedKernel;
using Ardalis.Specification;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Get;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.Get;

[TestFixture]
public class GetUserHandlerTests
{
    private IReadRepository<User> _repository = null!;
    private GetUserHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IReadRepository<User>>();
        _handler = new GetUserHandler(_repository);
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(new GetUserQuery(UserId.From(1)), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenUserExists_ReturnsMappedDto()
    {
        var user = new User(UserId.From(1), Email.From("user@example.com"), UserName.From("user"));
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _handler.Handle(new GetUserQuery(UserId.From(1)), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(UserId.From(1));
        result.Value.EmailAddress.ShouldBe(Email.From("user@example.com"));
        result.Value.Name.ShouldBe(UserName.From("user"));
    }
}
