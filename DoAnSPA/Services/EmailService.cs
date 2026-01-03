using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using DoAnSPA.Models;

namespace DoAnSPA.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MimeKit.MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new MimeKit.BodyBuilder { HtmlBody = htmlBody };
            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();

            await client.ConnectAsync(
                _settings.SmtpServer,
                _settings.SmtpPort,
                MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(
                _settings.SmtpUsername, 
                _settings.SmtpPassword);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

}
