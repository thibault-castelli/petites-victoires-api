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
    private const string EmailAddress = "user@example.com";
    private const string Password = "password";
    private static readonly Email UserEmail = Email.From(EmailAddress);

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
        return new SignInCommand(UserEmail, Password);
    }

    [Test]
    public async Task Handle_WhenCredentialsValid_ReturnsAuthenticatedUserFromIdentity()
    {
        var authenticated = new AuthenticatedUser(1, "user", EmailAddress);
        _identityService.ValidateCredentialsAsync(UserEmail, Password, CancellationToken.None)
            .Returns(Result.Success(authenticated));

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(authenticated);
    }

    [Test]
    public async Task Handle_WhenCredentialsInvalid_ReturnsUnauthorizedFromIdentity()
    {
        _identityService.ValidateCredentialsAsync(UserEmail, Password, CancellationToken.None)
            .Returns(Result<AuthenticatedUser>.Unauthorized());

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }
}
