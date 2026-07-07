using FluentValidation.TestHelper;
using PetitesVictoires.Api.Posts.Update;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.UnitTests.Api.Posts.Update;

[TestFixture]
public class UpdatePostValidatorTests
{
    private readonly UpdatePostValidator _validator = new();

    private static UpdatePostRequest Request(int postId = 1, string content = "content")
    {
        return new UpdatePostRequest { PostId = postId, PostContent = content };
    }

    [Test]
    public void ValidRequest_HasNoErrors()
    {
        _validator.TestValidate(Request()).ShouldNotHaveAnyValidationErrors();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositivePostId_HasError(int postId)
    {
        _validator.TestValidate(Request(postId: postId)).ShouldHaveValidationErrorFor(r => r.PostId);
    }

    [Test]
    public void EmptyContent_HasRequiredMessage()
    {
        _validator.TestValidate(Request(content: ""))
            .ShouldHaveValidationErrorFor(r => r.PostContent)
            .WithErrorMessage("Content is required");
    }

    [Test]
    public void ContentAtMaxLength_HasNoErrors()
    {
        _validator.TestValidate(Request(content: new string('a', PostContent.MaxLength)))
            .ShouldNotHaveValidationErrorFor(r => r.PostContent);
    }

    [Test]
    public void ContentLongerThanMaxLength_HasError()
    {
        _validator.TestValidate(Request(content: new string('a', PostContent.MaxLength + 1)))
            .ShouldHaveValidationErrorFor(r => r.PostContent);
    }
}
