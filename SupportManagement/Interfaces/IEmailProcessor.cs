using MimeKit;

namespace SupportManagement.Interfaces;

public interface IEmailProcessor
{
    Task ProcessAsync(MimeMessage message);
}
