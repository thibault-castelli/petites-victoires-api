using Ardalis.Result;
using Ardalis.SharedKernel;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Users.Update;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.Update;

[TestFixture]
public class UpdateUserHandlerTests
{
    private static readonly UserId TargetUserId = UserId.From(1);
    private static readonly Email NewEmail = Email.From("new@example.com");
    private static readonly UserName NewName = UserName.From("newname");
    private const string CurrentPassword = "old";
    private const string NewPassword = "new";

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IRepository<User>>();
        _identityService = Substitute.For<IIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _cache = Substitute.For<IDistributedCache>();
        _handler = new UpdateUserHandler(_repository, _identityService, _unitOfWork, _cache);
    }

    private IRepository<User> _repository = null!;
    private IIdentityService _identityService = null!;
    private IUnitOfWork _unitOfWork = null!;
    private IDistributedCache _cache = null!;
    private UpdateUserHandler _handler = null!;

    private ITransaction ArrangeTransaction()
    {
        var transaction = Substitute.For<ITransaction>();
        _unitOfWork.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        return transaction;
    }

    private static User ExistingUser()
    {
        return new User(TargetUserId, Email.From("old@example.com"), UserName.From("old"));
    }

    private static UpdateUserCommand Command(string? currentPassword = null, string? newPassword = null)
    {
        return new UpdateUserCommand(TargetUserId, NewEmail, NewName, currentPassword, newPassword);
    }

    private void ArrangeUserMissing()
    {
        _repository.GetByIdAsync(TargetUserId, CancellationToken.None).Returns((User?)null);
    }

    private void ArrangeIdentityUpdateFailure()
    {
        _repository.GetByIdAsync(TargetUserId, CancellationToken.None).Returns(ExistingUser());
        _identityService.UpdateUserAsync(TargetUserId, NewEmail, NewName).Returns(Result.Error("identity failed"));
    }

    private void ArrangePasswordChangeFailure()
    {
        _repository.GetByIdAsync(TargetUserId, CancellationToken.None).Returns(ExistingUser());
        _identityService.UpdateUserAsync(TargetUserId, NewEmail, NewName).Returns(Result.Success());
        _identityService.ChangePasswordAsync(TargetUserId, CurrentPassword, NewPassword)
            .Returns(Result.Error("wrong password"));
    }

    private User ArrangeValidWithoutPasswordChange()
    {
        var user = ExistingUser();
        _repository.GetByIdAsync(TargetUserId, CancellationToken.None).Returns(user);
        _identityService.UpdateUserAsync(TargetUserId, NewEmail, NewName).Returns(Result.Success());
        return user;
    }

    private User ArrangeValidWithPasswordChange()
    {
        var user = ArrangeValidWithoutPasswordChange();
        _identityService.ChangePasswordAsync(TargetUserId, CurrentPassword, NewPassword).Returns(Result.Success());
        return user;
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        ArrangeUserMissing();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_DoesNotPersist()
    {
        ArrangeUserMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_DoesNotRemoveFromCache()
    {
        ArrangeUserMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentityUpdateFails_ReturnsError()
    {
        ArrangeTransaction();
        ArrangeIdentityUpdateFailure();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
    }

    [Test]
    public async Task Handle_WhenIdentityUpdateFails_DoesNotPersist()
    {
        ArrangeTransaction();
        ArrangeIdentityUpdateFailure();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentityUpdateFails_DoesNotCommit()
    {
        var transaction = ArrangeTransaction();
        ArrangeIdentityUpdateFailure();

        await _handler.Handle(Command(), CancellationToken.None);

        await transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentityUpdateFails_DoesNotRemoveFromCache()
    {
        ArrangeTransaction();
        ArrangeIdentityUpdateFailure();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPasswordChangeFails_ReturnsError()
    {
        ArrangeTransaction();
        ArrangePasswordChangeFailure();

        var result = await _handler.Handle(Command(CurrentPassword, NewPassword), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
    }

    [Test]
    public async Task Handle_WhenPasswordChangeFails_DoesNotPersist()
    {
        ArrangeTransaction();
        ArrangePasswordChangeFailure();

        await _handler.Handle(Command(CurrentPassword, NewPassword), CancellationToken.None);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPasswordChangeFails_DoesNotCommit()
    {
        var transaction = ArrangeTransaction();
        ArrangePasswordChangeFailure();

        await _handler.Handle(Command(CurrentPassword, NewPassword), CancellationToken.None);

        await transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenPasswordChangeFails_DoesNotRemoveFromCache()
    {
        ArrangeTransaction();
        ArrangePasswordChangeFailure();

        await _handler.Handle(Command(CurrentPassword, NewPassword), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValidWithoutPasswordChange_ReturnsMappedDto()
    {
        ArrangeTransaction();
        ArrangeValidWithoutPasswordChange();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.EmailAddress.ShouldBe(NewEmail);
        result.Value.Name.ShouldBe(NewName);
    }

    [Test]
    public async Task Handle_WhenValidWithoutPasswordChange_DoesNotChangePassword()
    {
        ArrangeTransaction();
        ArrangeValidWithoutPasswordChange();

        await _handler.Handle(Command(), CancellationToken.None);

        await _identityService.DidNotReceiveWithAnyArgs().ChangePasswordAsync(TargetUserId, null!, null!);
    }

    [Test]
    public async Task Handle_WhenValidWithoutPasswordChange_PersistsTheUser()
    {
        ArrangeTransaction();
        var user = ArrangeValidWithoutPasswordChange();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.Received(1).UpdateAsync(user, CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenValidWithoutPasswordChange_CommitsTheTransaction()
    {
        var transaction = ArrangeTransaction();
        ArrangeValidWithoutPasswordChange();

        await _handler.Handle(Command(), CancellationToken.None);

        await transaction.Received(1).CommitAsync(CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenValidWithoutPasswordChange_RemovesFromCache()
    {
        ArrangeTransaction();
        ArrangeValidWithoutPasswordChange();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1)
            .RemoveAsync($"{Constants.UserCachePrefix}{TargetUserId.Value}", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValidWithPasswordChange_ReturnsSuccess()
    {
        ArrangeTransaction();
        ArrangeValidWithPasswordChange();

        var result = await _handler.Handle(Command(CurrentPassword, NewPassword), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Handle_WhenValidWithPasswordChange_ChangesThePassword()
    {
        ArrangeTransaction();
        ArrangeValidWithPasswordChange();

        await _handler.Handle(Command(CurrentPassword, NewPassword), CancellationToken.None);

        await _identityService.Received(1).ChangePasswordAsync(TargetUserId, CurrentPassword, NewPassword);
    }

    [Test]
    public async Task Handle_WhenValidWithPasswordChange_PersistsTheUser()
    {
        ArrangeTransaction();
        var user = ArrangeValidWithPasswordChange();

        await _handler.Handle(Command(CurrentPassword, NewPassword), CancellationToken.None);

        await _repository.Received(1).UpdateAsync(user, CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenValidWithPasswordChange_CommitsTheTransaction()
    {
        var transaction = ArrangeTransaction();
        ArrangeValidWithPasswordChange();

        await _handler.Handle(Command(CurrentPassword, NewPassword), CancellationToken.None);

        await transaction.Received(1).CommitAsync(CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenValidWithPasswordChange_RemovesFromCache()
    {
        ArrangeTransaction();
        ArrangeValidWithPasswordChange();

        await _handler.Handle(Command(CurrentPassword, NewPassword), CancellationToken.None);

        await _cache.Received(1)
            .RemoveAsync($"{Constants.UserCachePrefix}{TargetUserId.Value}", Arg.Any<CancellationToken>());
    }
}
