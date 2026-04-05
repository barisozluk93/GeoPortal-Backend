using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using SupportManagement.Interfaces;
using SupportManagement.Settings;

namespace SupportManagement.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly MailSettings _mailSettings;

    public SmtpEmailSender(IOptions<MailSettings> options)
    {
        _mailSettings = options.Value;
    }

    public async Task SendAsync(string toEmail, string subject, string body, string? inReplyTo = null)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        if (!string.IsNullOrWhiteSpace(inReplyTo))
        {
            message.InReplyTo = inReplyTo;
        }

        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, _mailSettings.SmtpUseSsl);
        await client.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
