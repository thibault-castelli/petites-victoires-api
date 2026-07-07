using Ardalis.Result;
using NSubstitute;
using PetitesVictoires.Core.Common;
using PetitesVictoires.Core.Interfaces;
using PetitesVictoires.UseCases.Users.SignIn;
using Shouldly;

namespace PetitesVictoires.UnitTests.UseCases.Users.SignIn;

[TestFixture]
public class SignInHandlerTests
{
    private IIdentityService _identityService = null!;
    private SignInHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _identityService = Substitute.For<IIdentityService>();
        _handler = new SignInHandler(_identityService);
    }

    private static SignInCommand Command()
    {
        return new SignInCommand(Email.From("user@example.com"), "password");
    }

    [Test]
    public async Task Handle_WhenCredentialsValid_ReturnsAuthenticatedUserFromIdentity()
    {
        var authenticated = new AuthenticatedUser(1, "user", "user@example.com");
        _identityService.ValidateCredentialsAsync(Email.From("user@example.com"), "password", CancellationToken.None)
            .Returns(Result.Success(authenticated));

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(authenticated);
    }

    [Test]
    public async Task Handle_WhenCredentialsInvalid_ReturnsUnauthorizedFromIdentity()
    {
        _identityService.ValidateCredentialsAsync(Email.From("user@example.com"), "password", CancellationToken.None)
            .Returns(Result<AuthenticatedUser>.Unauthorized());

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }
}
