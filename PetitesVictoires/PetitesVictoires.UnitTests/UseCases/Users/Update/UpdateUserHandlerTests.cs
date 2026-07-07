using Ardalis.Result;
using Ardalis.SharedKernel;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Update;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.Update;

[TestFixture]
public class UpdateUserHandlerTests
{
    private IRepository<User> _repository = null!;
    private IIdentityService _identityService = null!;
    private IUnitOfWork _unitOfWork = null!;
    private UpdateUserHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IRepository<User>>();
        _identityService = Substitute.For<IIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new UpdateUserHandler(_repository, _identityService, _unitOfWork);
    }

    private ITransaction ArrangeTransaction()
    {
        var transaction = Substitute.For<ITransaction>();
        _unitOfWork.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        return transaction;
    }

    private static User ExistingUser()
    {
        return new User(UserId.From(1), Email.From("old@example.com"), UserName.From("old"));
    }

    private static UpdateUserCommand Command(string? currentPassword = null, string? newPassword = null)
    {
        return new UpdateUserCommand(UserId.From(1), Email.From("new@example.com"), UserName.From("newname"),
            currentPassword, newPassword);
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        _repository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns((User?)null);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentityUpdateFails_ReturnsErrorWithoutPersistingOrCommitting()
    {
        var transaction = ArrangeTransaction();
        _repository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns(ExistingUser());
        _identityService.UpdateUserAsync(UserId.From(1), Email.From("new@example.com"), UserName.From("newname"))
            .Returns(Result.Error("identity failed"));

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPasswordChangeFails_ReturnsErrorWithoutPersistingOrCommitting()
    {
        var transaction = ArrangeTransaction();
        _repository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns(ExistingUser());
        _identityService.UpdateUserAsync(UserId.From(1), Email.From("new@example.com"), UserName.From("newname"))
            .Returns(Result.Success());
        _identityService.ChangePasswordAsync(UserId.From(1), "old", "new").Returns(Result.Error("wrong password"));

        var result = await _handler.Handle(Command(currentPassword: "old", newPassword: "new"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValidWithoutPasswordChange_UpdatesUserCommitsAndSkipsPasswordChange()
    {
        var transaction = ArrangeTransaction();
        var user = ExistingUser();
        _repository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns(user);
        _identityService.UpdateUserAsync(UserId.From(1), Email.From("new@example.com"), UserName.From("newname"))
            .Returns(Result.Success());

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.EmailAddress.ShouldBe(Email.From("new@example.com"));
        result.Value.Name.ShouldBe(UserName.From("newname"));
        await _identityService.DidNotReceiveWithAnyArgs().ChangePasswordAsync(UserId.From(1), null!, null!);
        await _repository.Received(1).UpdateAsync(user, CancellationToken.None);
        await transaction.Received(1).CommitAsync(CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenValidWithPasswordChange_ChangesPasswordCommitsAndReturnsDto()
    {
        var transaction = ArrangeTransaction();
        var user = ExistingUser();
        _repository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns(user);
        _identityService.UpdateUserAsync(UserId.From(1), Email.From("new@example.com"), UserName.From("newname"))
            .Returns(Result.Success());
        _identityService.ChangePasswordAsync(UserId.From(1), "old", "new").Returns(Result.Success());

        var result = await _handler.Handle(Command(currentPassword: "old", newPassword: "new"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _identityService.Received(1).ChangePasswordAsync(UserId.From(1), "old", "new");
        await _repository.Received(1).UpdateAsync(user, CancellationToken.None);
        await transaction.Received(1).CommitAsync(CancellationToken.None);
    }
}
