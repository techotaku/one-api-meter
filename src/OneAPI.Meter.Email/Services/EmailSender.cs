using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using OneAPI.Meter.Email.Models;
using OneAPI.Meter.Email.Options;

namespace OneAPI.Meter.Email.Services
{
    public class EmailSender (IOptions<EmailOptions> options, ILogger<EmailSender> logger)
    {
        private readonly EmailOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        private readonly ILogger<EmailSender> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task SendAsync(EmailReport emailReport)
        {
            try
            {
                var message = new MimeMessage();
                var sender = MailboxAddress.Parse(_options.From);
                message.From.Add(sender);
                message.ReplyTo.Add(sender);
                foreach (var to in _options.To)
                {
                    message.To.Add(MailboxAddress.Parse(to));
                }
                message.Subject = "Usage Report - " + _options.SiteName;
                message.Body = new TextPart("html")
                {
                    Text = emailReport.ToHtml(_options.SiteName)
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(_options.Host, _options.Port);
                await client.AuthenticateAsync(_options.User, _options.Pwd);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Statistics report has been sent to {recipients}.", string.Join(", ", _options.To.Select(t => $"\"{t}\"")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending usage report.");
            }
        }
    }
}
