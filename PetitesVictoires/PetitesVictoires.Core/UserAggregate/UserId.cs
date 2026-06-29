using Vogen;

namespace PetitesVictoires.Core.UserAggregate;

[ValueObject<int>]
public readonly partial struct UserId
{
    private static Validation Validate(int value)
    {
        return value > 0 ? Validation.Ok : Validation.Invalid("UserId must be positive.");
    }
}
