namespace PetitesVictoires.Infrastructure.Emailing;

public class MailSettings
{
    public const string SectionName = "MailSettings";

    public string From { get; set; } = "no-reply@petitesvictoires.app";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1025;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool UseSsl { get; set; }
}
