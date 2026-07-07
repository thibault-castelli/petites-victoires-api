using FluentValidation.TestHelper;
using PetitesVictoires.Api.Posts.Create;
using PetitesVictoires.Core.PostAggregate;

namespace PetitesVictoires.UnitTests.Api.Posts.Create;

[TestFixture]
public class CreatePostValidatorTests
{
    private readonly CreatePostValidator _validator = new();

    private static CreatePostRequest Request(string content = "content")
    {
        return new CreatePostRequest { Content = content };
    }

    [Test]
    public void ValidContent_HasNoErrors()
    {
        _validator.TestValidate(Request()).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void EmptyContent_HasRequiredMessage()
    {
        _validator.TestValidate(Request(content: ""))
            .ShouldHaveValidationErrorFor(r => r.Content)
            .WithErrorMessage("Content is required");
    }

    [Test]
    public void ContentAtMaxLength_HasNoErrors()
    {
        _validator.TestValidate(Request(content: new string('a', PostContent.MaxLength)))
            .ShouldNotHaveValidationErrorFor(r => r.Content);
    }

    [Test]
    public void ContentLongerThanMaxLength_HasError()
    {
        _validator.TestValidate(Request(content: new string('a', PostContent.MaxLength + 1)))
            .ShouldHaveValidationErrorFor(r => r.Content);
    }
}
