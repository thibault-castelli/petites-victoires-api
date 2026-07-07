using Ardalis.Result;
using Ardalis.SharedKernel;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Create;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.Create;

[TestFixture]
public class CreateUserHandlerTests
{
    private IRepository<User> _repository = null!;
    private IIdentityService _identityService = null!;
    private IUnitOfWork _unitOfWork = null!;
    private CreateUserHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IRepository<User>>();
        _identityService = Substitute.For<IIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateUserHandler(_repository, _identityService, _unitOfWork);
    }

    // Kept as a local (not a field) so the substitute isn't treated as a disposable fixture member.
    private ITransaction ArrangeTransaction()
    {
        var transaction = Substitute.For<ITransaction>();
        _unitOfWork.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        return transaction;
    }

    private static CreateUserCommand Command()
    {
        return new CreateUserCommand(Email.From("new@example.com"), UserName.From("newbie"), " secret ");
    }

    [Test]
    public async Task Handle_WhenIdentityCreationFails_ReturnsErrorWithoutPersistingOrCommitting()
    {
        var transaction = ArrangeTransaction();
        _identityService
            .CreateUserAsync(Email.From("new@example.com"), UserName.From("newbie"), "secret", CancellationToken.None)
            .Returns(Result<UserId>.Error("identity failed"));

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
        await _repository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentitySucceeds_PersistsUserCommitsAndReturnsId()
    {
        var transaction = ArrangeTransaction();
        var newUserId = UserId.From(5);
        _identityService
            .CreateUserAsync(Email.From("new@example.com"), UserName.From("newbie"), "secret", CancellationToken.None)
            .Returns(Result.Success(newUserId));
        _repository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(ci => ci.Arg<User>());

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(newUserId);
        await _repository.Received(1).AddAsync(Arg.Is<User>(u => u.Id == newUserId), Arg.Any<CancellationToken>());
        await transaction.Received(1).CommitAsync(CancellationToken.None);
    }

    [Test]
    public async Task Handle_TrimsPasswordBeforePassingToIdentity()
    {
        ArrangeTransaction();
        _identityService
            .CreateUserAsync(Email.From("new@example.com"), UserName.From("newbie"), "secret", CancellationToken.None)
            .Returns(Result.Success(UserId.From(5)));
        _repository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(ci => ci.Arg<User>());

        await _handler.Handle(Command(), CancellationToken.None);

        await _identityService.Received(1)
            .CreateUserAsync(Email.From("new@example.com"), UserName.From("newbie"), "secret", CancellationToken.None);
    }
}
