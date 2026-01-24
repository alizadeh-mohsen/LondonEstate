using LondonEstate.Models;
using LondonEstate.Utils.Types;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;
using System.Text;

namespace LondonEstate.Services
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public MailKitEmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        //public async Task SendAsync(Customer customer, Property property, string? attachmentFilePath)
        public async Task SendEstimateRequestEmailAsync(Customer customer, Property property)
        {
            if (customer is null) throw new ArgumentNullException(nameof(customer));
            if (property is null) throw new ArgumentNullException(nameof(property));

            // Build subject
            var subject = $"" +
                $"Estimate-{customer.Name ?? customer.Email}";

            // Build HTML body
            var sb = new StringBuilder();
            sb.AppendLine("<!doctype html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"utf-8\" />");
            sb.AppendLine("</head>");
            sb.AppendLine("<body style=\"font-family:Segoe UI, Arial, sans-serif; color:#111; line-height:1.4;\">");
            //sb.AppendLine("<h2 style=\"margin-bottom:0.25rem;\">A new property has been submitted</h2>");
            //sb.AppendLine("<p style=\"color:#555; margin-top:0.25rem;\">Details are shown below.</p>");

            // Customer section
            //sb.AppendLine("<h3 style=\"border-bottom:1px solid #eee; padding-bottom:0.25rem;\">Customer</h3>");
            sb.AppendLine("<table style=\"border-collapse:collapse; width:100%; max-width:600px;\">");
            AppendRow(sb, "Name", customer.Name);
            AppendRow(sb, "Email", customer.Email);
            AppendRow(sb, "Phone Number", customer.PhoneNumber);
            //sb.AppendLine("</table>");

            // Property section
            //sb.AppendLine("<h3 style=\"border-bottom:1px solid #eee; padding-bottom:0.25rem; margin-top:1rem;\">Property</h3>");
            //sb.AppendLine("<table style=\"border-collapse:collapse; width:100%; max-width:600px;\">");
            AppendRow(sb, "Address", property.Address);
            AppendRow(sb, "Number Of Beds", property.NumberOfBeds.ToString());
            AppendRow(sb, "Square Meter", property.SquareMeter.ToString());
            sb.AppendLine("</table>");

            //sb.AppendLine("<p style=\"color:#888; font-size:0.9rem; margin-top:1rem;\">This message was generated automatically.</p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            string htmlBody = sb.ToString();

            // Create message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            email.To.Add(MailboxAddress.Parse(_settings.ToEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            // Attach file if provided and exists
            //if (!string.IsNullOrWhiteSpace(attachmentFilePath) && File.Exists(attachmentFilePath))
            //{
            //    var fileName = Path.GetFileName(attachmentFilePath);
            //    builder.Attachments.Add(fileName, File.ReadAllBytes(attachmentFilePath));
            //}

            email.Body = builder.ToMessageBody();

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

            // Local helper to append a table row with simple styling and HTML-encoding
            static void AppendRow(StringBuilder sbLocal, string label, string? value)
            {
                var safeLabel = WebUtility.HtmlEncode(label ?? string.Empty);
                var safeValue = WebUtility.HtmlEncode(value ?? string.Empty);
                sbLocal.AppendLine("<tr>");
                sbLocal.AppendLine($"  <td style=\"padding:6px 8px; font-weight:600; vertical-align:top; width:160px; color:#222;\">{safeLabel}</td>");
                sbLocal.AppendLine($"  <td style=\"padding:6px 8px; vertical-align:top; color:#333;\">{safeValue}</td>");
                sbLocal.AppendLine("</tr>");
            }
        }
    }
}
