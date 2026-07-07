using FluentValidation.TestHelper;
using PetitesVictoires.Api.Posts.Get;

namespace PetitesVictoires.UnitTests.Api.Posts.Get;

[TestFixture]
public class GetPostByIdValidatorTests
{
    private readonly GetPostByIdValidator _validator = new();

    [Test]
    public void PositivePostId_HasNoErrors()
    {
        _validator.TestValidate(new GetPostByIdRequest { PostId = 1 }).ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositivePostId_HasError(int postId)
    {
        _validator.TestValidate(new GetPostByIdRequest { PostId = postId })
            .ShouldHaveValidationErrorFor(r => r.PostId);
    }
}
