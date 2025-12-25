using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace WebApplication2.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _options;

        public EmailSender(IOptions<SmtpSettings> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_options.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlContent
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            if (_options.UseSsl)
            {
                await client.ConnectAsync(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);
            }
            else
            {
                await client.ConnectAsync(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTls);
            }

            if (!string.IsNullOrEmpty(_options.UserName))
            {
                await client.AuthenticateAsync(_options.UserName, _options.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}