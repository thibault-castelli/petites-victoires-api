using Vogen;

namespace PetitesVictoires.Core.PostAggregate;

[ValueObject<int>]
public readonly partial struct PostId
{
    private static Validation Validate(int value)
    {
        return value > 0 ? Validation.Ok : Validation.Invalid("PostId must be positive.");
    }
}
