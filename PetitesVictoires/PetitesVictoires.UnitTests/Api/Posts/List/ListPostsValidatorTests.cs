using FluentValidation.TestHelper;
using PetitesVictoires.Api.Posts.List;
using PetitesVictoires.UseCases;

namespace PetitesVictoires.UnitTests.Api.Posts.List;

[TestFixture]
public class ListPostsValidatorTests
{
    private readonly ListPostsValidator _validator = new();

    private static ListPostsRequest Request(
        int page = 1,
        int countPerPage = 10,
        string? sortBy = null,
        int? likedBy = null,
        int? createdBy = null)
    {
        return new ListPostsRequest
        {
            Page = page,
            CountPerPage = countPerPage,
            SortBy = sortBy,
            LikedBy = likedBy,
            CreatedBy = createdBy
        };
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

    [TestCase("created_at")]
    [TestCase("likes_count")]
    public void KnownSortBy_HasNoErrors(string sortBy)
    {
        _validator.TestValidate(Request(sortBy: sortBy)).ShouldNotHaveValidationErrorFor(r => r.SortBy);
    }

    [Test]
    public void NullSortBy_HasNoErrors()
    {
        _validator.TestValidate(Request(sortBy: null)).ShouldNotHaveValidationErrorFor(r => r.SortBy);
    }

    [Test]
    public void UnknownSortBy_HasCustomMessage()
    {
        _validator.TestValidate(Request(sortBy: "unknown"))
            .ShouldHaveValidationErrorFor(r => r.SortBy)
            .WithErrorMessage(
                $"Sort by must be one of the following: {string.Join(", ", ListPostsValidator.AllowedPostsSorts)}");
    }

    [Test]
    public void LikedByZero_HasError()
    {
        _validator.TestValidate(Request(likedBy: 0)).ShouldHaveValidationErrorFor(r => r.LikedBy);
    }

    [Test]
    public void LikedByPositive_HasNoErrors()
    {
        _validator.TestValidate(Request(likedBy: 1)).ShouldNotHaveValidationErrorFor(r => r.LikedBy);
    }

    [Test]
    public void LikedByNull_HasNoErrors()
    {
        _validator.TestValidate(Request(likedBy: null)).ShouldNotHaveValidationErrorFor(r => r.LikedBy);
    }

    [Test]
    public void CreatedByZero_HasError()
    {
        _validator.TestValidate(Request(createdBy: 0)).ShouldHaveValidationErrorFor(r => r.CreatedBy);
    }

    [Test]
    public void CreatedByPositive_HasNoErrors()
    {
        _validator.TestValidate(Request(createdBy: 1)).ShouldNotHaveValidationErrorFor(r => r.CreatedBy);
    }

    [Test]
    public void CreatedByNull_HasNoErrors()
    {
        _validator.TestValidate(Request(createdBy: null)).ShouldNotHaveValidationErrorFor(r => r.CreatedBy);
    }
}
