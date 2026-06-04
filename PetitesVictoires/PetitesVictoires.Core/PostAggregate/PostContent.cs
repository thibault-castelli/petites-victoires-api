using Vogen;

namespace PetitesVictoires.Core.PostAggregate;

[ValueObject<string>]
public readonly partial struct PostContent
{
    public const int MaxLength = 5000;

    private static Validation Validate(in string content)
    {
        if (string.IsNullOrEmpty(content)) return Validation.Invalid("Content cannot be empty");

        return content.Length > MaxLength
            ? Validation.Invalid($"Content cannot be longer than {MaxLength} characters")
            : Validation.Ok;
    }
}
