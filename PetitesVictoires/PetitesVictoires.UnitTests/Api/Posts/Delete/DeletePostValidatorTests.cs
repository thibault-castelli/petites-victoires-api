using FluentValidation.TestHelper;
using PetitesVictoires.Api.Posts.Delete;

namespace PetitesVictoires.UnitTests.Api.Posts.Delete;

[TestFixture]
public class DeletePostValidatorTests
{
    private readonly DeletePostValidator _validator = new();

    [Test]
    public void PositivePostId_HasNoErrors()
    {
        _validator.TestValidate(new DeletePostRequest { PostId = 1 }).ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositivePostId_HasError(int postId)
    {
        _validator.TestValidate(new DeletePostRequest { PostId = postId })
            .ShouldHaveValidationErrorFor(r => r.PostId);
    }
}
