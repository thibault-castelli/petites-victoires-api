using FastEndpoints;
using FluentValidation;

namespace PetitesVictoires.Api.Posts.GetPostById;

public class GetPostByIdValidator : Validator<GetPostByIdRequest>
{
    public GetPostByIdValidator()
    {
        RuleFor(r => r.PostId)
            .GreaterThan(0);
    }
}
