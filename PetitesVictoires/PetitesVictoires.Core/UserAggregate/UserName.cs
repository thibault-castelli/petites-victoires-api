using Vogen;

namespace PetitesVictoires.Core.UserAggregate;

[ValueObject<string>]
public readonly partial struct UserName
{
    public const int MaxLength = 32;

    private static string NormalizeInput(string input)
    {
        return input.Trim();
    }

    private static Validation Validate(in string name)
    {
        if (string.IsNullOrEmpty(name)) return Validation.Invalid("Name cannot be empty");

        return name.Length > MaxLength
            ? Validation.Invalid($"Name cannot be longer than {MaxLength} characters")
            : Validation.Ok;
    }
}
