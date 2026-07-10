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
    private static readonly UserId TargetUserId = UserId.From(1);
    private static readonly Email UserEmail = Email.From("user@example.com");
    private static readonly UserName Name = UserName.From("user");

    private IReadRepository<User> _repository = null!;
    private GetUserHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IReadRepository<User>>();
        _handler = new GetUserHandler(_repository);
    }

    private static GetUserQuery Query()
    {
        return new GetUserQuery(TargetUserId);
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(Query(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenUserExists_ReturnsMappedDto()
    {
        var user = new User(TargetUserId, UserEmail, Name);
        _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _handler.Handle(Query(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(TargetUserId);
        result.Value.EmailAddress.ShouldBe(UserEmail);
        result.Value.Name.ShouldBe(Name);
    }
}
