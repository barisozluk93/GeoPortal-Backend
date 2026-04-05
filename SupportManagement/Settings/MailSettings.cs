namespace SupportManagement.Settings;

public class MailSettings
{
    public string ImapHost { get; set; } = string.Empty;
    public int ImapPort { get; set; }
    public bool ImapUseSsl { get; set; }
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public bool SmtpUseSsl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public int PollingIntervalSeconds { get; set; } = 30;
}
