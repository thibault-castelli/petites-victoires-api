using Vogen;

namespace PetitesVictoires.Core.LikeAggregate;

[ValueObject<int>(deserializationStrictness: DeserializationStrictness.AllowAnything)]
public readonly partial struct LikeId
{
    private static Validation Validate(int value)
    {
        return value > 0 ? Validation.Ok : Validation.Invalid("LikeId must be positive");
    }
}
