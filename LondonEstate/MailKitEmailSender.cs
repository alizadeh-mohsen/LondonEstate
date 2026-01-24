using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LondonEstate
{

    public class MailKitEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public MailKitEmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }

}
