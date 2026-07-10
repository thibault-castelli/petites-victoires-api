using Ardalis.Result;
using Ardalis.SharedKernel;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases;
using PetitesVictoires.UseCases.Users.Delete;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.Delete;

[TestFixture]
public class DeleteUserHandlerTests
{
    private static readonly UserId TargetUserId = UserId.From(1);

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IRepository<User>>();
        _identityService = Substitute.For<IIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _cache = Substitute.For<IDistributedCache>();
        _handler = new DeleteUserHandler(_repository, _identityService, _unitOfWork, _cache);
    }

    private IRepository<User> _repository = null!;
    private IIdentityService _identityService = null!;
    private IUnitOfWork _unitOfWork = null!;
    private IDistributedCache _cache = null!;
    private DeleteUserHandler _handler = null!;

    private ITransaction ArrangeTransaction()
    {
        var transaction = Substitute.For<ITransaction>();
        _unitOfWork.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        return transaction;
    }

    private static User ExistingUser()
    {
        return new User(TargetUserId, Email.From("user@example.com"), UserName.From("user"));
    }

    private static DeleteUserCommand Command()
    {
        return new DeleteUserCommand(TargetUserId);
    }

    private void ArrangeUserMissing()
    {
        _repository.GetByIdAsync(TargetUserId, CancellationToken.None).Returns((User?)null);
    }

    private void ArrangeIdentityFailure()
    {
        _repository.GetByIdAsync(TargetUserId, CancellationToken.None).Returns(ExistingUser());
        _identityService.DeleteUserAsync(TargetUserId, CancellationToken.None)
            .Returns(Result.Error("identity failed"));
    }

    private User ArrangeValid()
    {
        var user = ExistingUser();
        _repository.GetByIdAsync(TargetUserId, CancellationToken.None).Returns(user);
        _identityService.DeleteUserAsync(TargetUserId, CancellationToken.None).Returns(Result.Success());
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
    public async Task Handle_WhenUserDoesNotExist_DoesNotDeleteUser()
    {
        ArrangeUserMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.DidNotReceive().DeleteAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_DoesNotDeleteCache()
    {
        ArrangeUserMissing();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentityDeleteFails_ReturnsError()
    {
        ArrangeTransaction();
        ArrangeIdentityFailure();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
    }

    [Test]
    public async Task Handle_WhenIdentityDeleteFails_DoesNotDeleteUser()
    {
        ArrangeTransaction();
        ArrangeIdentityFailure();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.DidNotReceive().DeleteAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentityDeleteFails_DoesNotCommit()
    {
        var transaction = ArrangeTransaction();
        ArrangeIdentityFailure();

        await _handler.Handle(Command(), CancellationToken.None);

        await transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentityDeleteFails_DoesNotDeleteCache()
    {
        ArrangeTransaction();
        ArrangeIdentityFailure();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValid_ReturnsSuccess()
    {
        ArrangeTransaction();
        ArrangeValid();

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Test]
    public async Task Handle_WhenValid_DeletesTheUser()
    {
        ArrangeTransaction();
        var user = ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        await _repository.Received(1).DeleteAsync(user, CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenValid_CommitsTheTransaction()
    {
        var transaction = ArrangeTransaction();
        ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        await transaction.Received(1).CommitAsync(CancellationToken.None);
    }

    [Test]
    public async Task Handle_WhenValid_DeletesCache()
    {
        ArrangeTransaction();
        ArrangeValid();

        await _handler.Handle(Command(), CancellationToken.None);

        await _cache.Received(1)
            .RemoveAsync($"{Constants.UserCachePrefix}{TargetUserId.Value}", Arg.Any<CancellationToken>());
    }
}
