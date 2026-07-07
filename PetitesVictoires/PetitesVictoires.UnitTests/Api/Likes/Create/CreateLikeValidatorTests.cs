using FluentValidation.TestHelper;
using PetitesVictoires.Api.Likes.Create;

namespace PetitesVictoires.UnitTests.Api.Likes.Create;

[TestFixture]
public class CreateLikeValidatorTests
{
    private readonly CreateLikeValidator _validator = new();

    [Test]
    public void PositivePostId_HasNoErrors()
    {
        _validator.TestValidate(new CreateLikeRequest { PostId = 1 }).ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositivePostId_HasError(int postId)
    {
        _validator.TestValidate(new CreateLikeRequest { PostId = postId })
            .ShouldHaveValidationErrorFor(r => r.PostId);
    }
}
