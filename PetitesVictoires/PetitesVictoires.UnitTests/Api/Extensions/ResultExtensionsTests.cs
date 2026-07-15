using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using PetitesVictoires.Api.Extensions;
using Shouldly;

namespace PetitesVictoires.UnitTests.Api.Extensions;

[TestFixture]
public class ResultExtensionsTests
{
    [Test]
    public void ToUpdateResultWithForbidden_WhenOk_ReturnsOkWithMappedResponse()
    {
        var mapped = Result<string>.Success("value").ToUpdateResultWithForbidden(v => $"mapped:{v}");

        mapped.Result.ShouldBeOfType<Ok<string>>().Value.ShouldBe("mapped:value");
    }

    [Test]
    public void ToUpdateResultWithForbidden_WhenNotFound_ReturnsNotFound()
    {
        var mapped = Result<string>.NotFound().ToUpdateResultWithForbidden(v => v);

        mapped.Result.ShouldBeOfType<NotFound>();
    }

    [Test]
    public void ToUpdateResultWithForbidden_WhenForbidden_ReturnsForbid()
    {
        var mapped = Result<string>.Forbidden().ToUpdateResultWithForbidden(v => v);

        mapped.Result.ShouldBeOfType<ForbidHttpResult>();
    }

    [Test]
    public void ToUpdateResultWithForbidden_WhenError_ReturnsProblem400WithJoinedErrors()
    {
        var mapped = Result<string>.Error("boom").ToUpdateResultWithForbidden(v => v);

        var problem = mapped.Result.ShouldBeOfType<ProblemHttpResult>();
        problem.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        problem.ProblemDetails.Detail.ShouldContain("boom");
    }

    [Test]
    public void ToCreatedResult_WhenOk_ReturnsCreatedWithLocationAndBody()
    {
        var mapped = Result<string>.Success("42").ToCreatedResult(v => $"/posts/{v}", v => $"body:{v}");

        var created = mapped.Result.ShouldBeOfType<Created<string>>();
        created.Location.ShouldBe("/posts/42");
        created.Value.ShouldBe("body:42");
    }

    [Test]
    public void ToCreatedResult_WhenInvalid_ReturnsValidationProblemGroupedByIdentifier()
    {
        var result = Result<string>.Invalid(
            new ValidationError { Identifier = "Email", ErrorMessage = "Email is required" });

        var mapped = result.ToCreatedResult(v => v, v => v);

        var validation = mapped.Result.ShouldBeOfType<ValidationProblem>();
        validation.ProblemDetails.Errors.ShouldContainKey("Email");
        validation.ProblemDetails.Errors["Email"].ShouldContain("Email is required");
    }

    [Test]
    public void ToCreatedResult_WhenError_ReturnsProblem400()
    {
        var mapped = Result<string>.Error("nope").ToCreatedResult(v => v, v => v);

        mapped.Result.ShouldBeOfType<ProblemHttpResult>().StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Test]
    public void ToCreatedResult_WhenNotFound_ReturnsProblem404()
    {
        var mapped = Result<string>.NotFound("missing").ToCreatedResult(v => v, v => v);

        mapped.Result.ShouldBeOfType<ProblemHttpResult>().StatusCode.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Test]
    public void ToCreatedResult_WhenConflict_ReturnsProblem409()
    {
        var mapped = Result<string>.Conflict("already exists").ToCreatedResult(v => v, v => v);

        mapped.Result.ShouldBeOfType<ProblemHttpResult>().StatusCode.ShouldBe(StatusCodes.Status409Conflict);
    }

    [Test]
    public void ToCreatedResult_WhenConflict_KeepsErrorsInDetail()
    {
        var mapped = Result<string>.Conflict("already exists").ToCreatedResult(v => v, v => v);

        mapped.Result.ShouldBeOfType<ProblemHttpResult>().ProblemDetails.Detail.ShouldContain("already exists");
    }

    [Test]
    public void ToGetByIdResult_WhenOk_ReturnsOkWithMappedResponse()
    {
        var mapped = Result<string>.Success("value").ToGetByIdResult(v => $"mapped:{v}");

        mapped.Result.ShouldBeOfType<Ok<string>>().Value.ShouldBe("mapped:value");
    }

    [Test]
    public void ToGetByIdResult_WhenNotFound_ReturnsNotFound()
    {
        var mapped = Result<string>.NotFound().ToGetByIdResult(v => v);

        mapped.Result.ShouldBeOfType<NotFound>();
    }

    [Test]
    public void ToGetByIdResult_WhenError_ReturnsProblemTitledGetFailed()
    {
        var mapped = Result<string>.Error("boom").ToGetByIdResult(v => v);

        mapped.Result.ShouldBeOfType<ProblemHttpResult>().ProblemDetails.Title.ShouldBe("Get failed");
    }

    [Test]
    public void ToUpdateResult_WhenError_ReturnsProblemTitledUpdateFailed()
    {
        var mapped = Result<string>.Error("boom").ToUpdateResult(v => v);

        mapped.Result.ShouldBeOfType<ProblemHttpResult>().ProblemDetails.Title.ShouldBe("Update failed");
    }

    [Test]
    public void ToDeleteResult_WhenOk_ReturnsNoContent()
    {
        var mapped = Result.Success().ToDeleteResult();

        mapped.Result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public void ToDeleteResult_WhenNotFound_ReturnsNotFound()
    {
        var mapped = Result.NotFound().ToDeleteResult();

        mapped.Result.ShouldBeOfType<NotFound>();
    }

    [Test]
    public void ToDeleteWithForbidResult_WhenForbidden_ReturnsForbid()
    {
        var mapped = Result.Forbidden().ToDeleteWithForbidResult();

        mapped.Result.ShouldBeOfType<ForbidHttpResult>();
    }

    [Test]
    public void ToDeleteWithForbidResult_WhenOk_ReturnsNoContent()
    {
        var mapped = Result.Success().ToDeleteWithForbidResult();

        mapped.Result.ShouldBeOfType<NoContent>();
    }

    [Test]
    public void ToOkOnlyResult_AlwaysReturnsOkWithMappedResponse()
    {
        var ok = Result<string>.Success("value").ToOkOnlyResult(v => $"mapped:{v}");

        ok.Value.ShouldBe("mapped:value");
    }
}
