using System.Net.Mail;
using Vogen;

namespace PetitesVictoires.Core.Common;

public readonly struct Email
{
    public const int MaxLength = 254; // RFC 5321 max length for an email address

    private static string NormalizeInput(string input)
    {
        return input.Trim();
    }

    private static Validation Validate(in string email)
    {
        if (string.IsNullOrEmpty(email))
            return Validation.Invalid("Email cannot be empty");

        if (email.Length > MaxLength)
            return Validation.Invalid($"Email cannot be longer than {MaxLength} characters");

        return MailAddress.TryCreate(email, out var parsed) && parsed.Address == email
            ? Validation.Ok
            : Validation.Invalid("Email is not a valid address");
    }
}
