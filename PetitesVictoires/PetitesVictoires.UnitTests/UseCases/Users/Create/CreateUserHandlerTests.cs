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
    private static readonly Email NewEmail = Email.From("new@example.com");
    private static readonly UserName NewName = UserName.From("newbie");
    private static readonly UserId NewUserId = UserId.From(5);
    private const string RawPassword = " secret ";
    private const string TrimmedPassword = "secret";

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
        return new CreateUserCommand(NewEmail, NewName, RawPassword);
    }

    private void ArrangeIdentityFailure()
    {
        _identityService.CreateUserAsync(NewEmail, NewName, TrimmedPassword, CancellationToken.None)
            .Returns(Result<UserId>.Error("identity failed"));
    }

    private void ArrangeIdentitySuccess()
    {
        _identityService.CreateUserAsync(NewEmail, NewName, TrimmedPassword, CancellationToken.None)
            .Returns(Result.Success(NewUserId));
        _repository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(ci => ci.Arg<User>());
    }

    [Test]
    public async Task Handle_WhenIdentityCreationFails_ReturnsError()
    {
        ArrangeTransaction();
        ArrangeIdentityFailure();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
    }

    [Test]
    public async Task Handle_WhenIdentityCreationFails_DoesNotPersistUser()
    {
        ArrangeTransaction();
        ArrangeIdentityFailure();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentityCreationFails_DoesNotCommit()
    {
        var transaction = ArrangeTransaction();
        ArrangeIdentityFailure();

        await _handler.Handle(Command(), CancellationToken.None);

        await transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentitySucceeds_ReturnsCreatedUserId()
    {
        ArrangeTransaction();
        ArrangeIdentitySuccess();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(NewUserId);
    }

    [Test]
    public async Task Handle_WhenIdentitySucceeds_PersistsTheUser()
    {
        ArrangeTransaction();
        ArrangeIdentitySuccess();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.Received(1).AddAsync(Arg.Is<User>(u => u.Id == NewUserId), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentitySucceeds_CommitsTheTransaction()
    {
        var transaction = ArrangeTransaction();
        ArrangeIdentitySuccess();

        await _handler.Handle(Command(), CancellationToken.None);

        await transaction.Received(1).CommitAsync(CancellationToken.None);
    }

    [Test]
    public async Task Handle_TrimsPasswordBeforePassingToIdentity()
    {
        ArrangeTransaction();
        ArrangeIdentitySuccess();

        await _handler.Handle(Command(), CancellationToken.None);

        await _identityService.Received(1)
            .CreateUserAsync(NewEmail, NewName, TrimmedPassword, CancellationToken.None);
    }
}
