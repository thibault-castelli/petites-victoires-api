using FluentValidation.TestHelper;
using PetitesVictoires.Api.Posts.List;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.UnitTests.Api.Posts.List;

[TestFixture]
public class ListPostsValidatorTests
{
    private readonly ListPostsValidator _validator = new();

    private static ListPostsRequest Request(int page = 1, int countPerPage = 10)
    {
        return new ListPostsRequest { Page = page, CountPerPage = countPerPage };
    }

    [Test]
    public void ValidRequest_HasNoErrors()
    {
        _validator.TestValidate(Request()).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void PageBelowOne_HasCustomMessage()
    {
        _validator.TestValidate(Request(page: 0))
            .ShouldHaveValidationErrorFor(r => r.Page)
            .WithErrorMessage("Page must be greater than 1");
    }

    [Test]
    public void CountPerPageBelowOne_HasError()
    {
        _validator.TestValidate(Request(countPerPage: 0)).ShouldHaveValidationErrorFor(r => r.CountPerPage);
    }

    [Test]
    public void CountPerPageAtMax_HasNoErrors()
    {
        _validator.TestValidate(Request(countPerPage: Constants.MaxPageSize))
            .ShouldNotHaveValidationErrorFor(r => r.CountPerPage);
    }

    [Test]
    public void CountPerPageAboveMax_HasCustomMessage()
    {
        _validator.TestValidate(Request(countPerPage: Constants.MaxPageSize + 1))
            .ShouldHaveValidationErrorFor(r => r.CountPerPage)
            .WithErrorMessage($"Count per page must be between 1 and {Constants.MaxPageSize}");
    }
}
