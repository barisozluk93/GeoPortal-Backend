using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Options;
using SupportManagement.Interfaces;
using SupportManagement.Settings;

namespace SupportManagement.Background;

public class MailListenerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MailSettings _mailSettings;
    private readonly ILogger<MailListenerService> _logger;

    public MailListenerService(IServiceProvider serviceProvider, IOptions<MailSettings> options, ILogger<MailListenerService> logger)
    {
        _serviceProvider = serviceProvider;
        _mailSettings = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Mail listener started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckMailboxAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking mailbox.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_mailSettings.PollingIntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("Mail listener stopped.");
    }

    private async Task CheckMailboxAsync(CancellationToken cancellationToken)
    {
        using var client = new ImapClient();

        await client.ConnectAsync(_mailSettings.ImapHost, _mailSettings.ImapPort, _mailSettings.ImapUseSsl, cancellationToken);
        await client.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password, cancellationToken);

        var inbox = client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);

        var unseenUids = await inbox.SearchAsync(SearchQuery.NotSeen, cancellationToken);

        foreach (var uid in unseenUids)
        {
            var message = await inbox.GetMessageAsync(uid, cancellationToken);

            using var scope = _serviceProvider.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IEmailProcessor>();
            await processor.ProcessAsync(message);

            await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken);
        }

        await client.DisconnectAsync(true, cancellationToken);
    }
}
