using FluentValidation.TestHelper;
using PetitesVictoires.Api.Likes.Delete;

namespace PetitesVictoires.UnitTests.Api.Likes.Delete;

[TestFixture]
public class DeleteLikeValidatorTests
{
    private readonly DeleteLikeValidator _validator = new();

    [Test]
    public void PositivePostId_HasNoErrors()
    {
        _validator.TestValidate(new DeleteLikeRequest { PostId = 1 }).ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositivePostId_HasError(int postId)
    {
        _validator.TestValidate(new DeleteLikeRequest { PostId = postId })
            .ShouldHaveValidationErrorFor(r => r.PostId);
    }
}
