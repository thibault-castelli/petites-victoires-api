using Ardalis.Result;
using Ardalis.SharedKernel;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.Core.UserAggregate;
using PetitesVictoires.UseCases.Users.Delete;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.Delete;

[TestFixture]
public class DeleteUserHandlerTests
{
    private IRepository<User> _repository = null!;
    private IIdentityService _identityService = null!;
    private IUnitOfWork _unitOfWork = null!;
    private DeleteUserHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IRepository<User>>();
        _identityService = Substitute.For<IIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new DeleteUserHandler(_repository, _identityService, _unitOfWork);
    }

    private ITransaction ArrangeTransaction()
    {
        var transaction = Substitute.For<ITransaction>();
        _unitOfWork.BeginTransactionAsync(CancellationToken.None).Returns(transaction);
        return transaction;
    }

    private static User ExistingUser()
    {
        return new User(UserId.From(1), Email.From("user@example.com"), UserName.From("user"));
    }

    [Test]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
    {
        _repository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns((User?)null);

        var result = await _handler.Handle(new DeleteUserCommand(UserId.From(1)), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenIdentityDeleteFails_ReturnsErrorWithoutDeletingOrCommitting()
    {
        var transaction = ArrangeTransaction();
        _repository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns(ExistingUser());
        _identityService.DeleteUserAsync(UserId.From(1), CancellationToken.None).Returns(Result.Error("identity failed"));

        var result = await _handler.Handle(new DeleteUserCommand(UserId.From(1)), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WhenValid_DeletesUserAndCommits()
    {
        var transaction = ArrangeTransaction();
        var user = ExistingUser();
        _repository.GetByIdAsync(UserId.From(1), CancellationToken.None).Returns(user);
        _identityService.DeleteUserAsync(UserId.From(1), CancellationToken.None).Returns(Result.Success());

        var result = await _handler.Handle(new DeleteUserCommand(UserId.From(1)), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _repository.Received(1).DeleteAsync(user, CancellationToken.None);
        await transaction.Received(1).CommitAsync(CancellationToken.None);
    }
}
