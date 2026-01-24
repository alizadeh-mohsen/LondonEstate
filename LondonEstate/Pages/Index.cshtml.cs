using LondonEstate.Data;
using LondonEstate.Models;
using LondonEstate.ModelViews;
//using System.Net.Mail;
//using System.Net;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;

namespace LondonEstate.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger<IndexModel> _logger;
        private readonly IEmailSender _emailSender;



        public IndexModel(IEmailSender emailSender, ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config, ILogger<IndexModel> logger)
        {
            _db = db;
            _env = env;
            _config = config;
            _logger = logger;
            _emailSender = emailSender;

        }

        [BindProperty]
        public CustomerViewModel Customer { get; set; } = new();

        [BindProperty]
        public PropertyViewModel Property { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // create customer
            var customer = new Models.Customer
            {
                Name = Customer.Name,
                Email = Customer.Email,
                CountryCode = Customer.CountryCode,
                PhoneNumber = Customer.Phone
            };

            _db.Customer.Add(customer);

            // create property
            var property = new Models.Property
            {
                Address = Property.Address,
                NumberOfBeds = Property.NumberOfBeds,
                SquareMeter = Property.SquareMeter,
                Customer = customer,
                CustomerId = customer.Id
            };

            _db.Property.Add(property);

            string? savedFilePath = null;

            // handle image
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var fileExt = Path.GetExtension(ImageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExt}";
                var filePath = Path.Combine(uploads, fileName);

                await using (var stream = System.IO.File.Create(filePath))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                savedFilePath = filePath;

                var imageRecord = new PropertyImage
                {
                    Property = property,
                    PropertyId = property.Id,
                    PicturePath = $"/uploads/{fileName}"
                };

                _db.PropertyImage.Add(imageRecord);
            }

            // attempt to send notification email with details and attachment (if any)
            try
            {
                await SendNotificationEmailAsync(customer, property, savedFilePath);
                //await SendEmailAsync("kelarens@gmail.com", "IONOS", "Mr. Watson — Come here — I want to see you.");
            }
            catch (Exception ex)
            {
                // Do not stop flow if email fails; log for later inspection
                _logger.LogError(ex, "Failed to send submission email.");
            }

            await _db.SaveChangesAsync();

            // redirect or show success
            return RedirectToPage("/Index");
        }

        private async Task SendNotificationEmailAsync(Customer customer, Property property, string? attachmentFilePath)
        {

            var bodyLines = new List<string>
            {
                "A new property have been submitted:",
                "",
                "Customer:",
                $"  Name: {customer.Name}",
                $"  Email: {customer.Email}",
                $"  PhoneNumber: {customer.PhoneNumber}",
                "",
                "Property:",
                $"  Address: {property.Address}",
                $"  NumberOfBeds: {property.NumberOfBeds}",
                $"  SquareMeter: {property.SquareMeter}",
            };

            //message.Body = string.Join(Environment.NewLine, bodyLines);

            //Attachment? attachment = null;
            //if (!string.IsNullOrEmpty(attachmentFilePath) && System.IO.File.Exists(attachmentFilePath))
            //{
            //    attachment = new Attachment(attachmentFilePath);
            //    message.Attachments.Add(attachment);
            //}

            //using var client = new SmtpClient(host, port)
            //{
            //    EnableSsl = enableSsl,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false // important: do not use machine credentials
            //};

            //client.Credentials = new NetworkCredential(username, password);

            try
            {
                //await client.SendMailAsync(message);
                await _emailSender.SendAsync(
               customer, property, attachmentFilePath
           );

            }
            catch (Exception smtpEx)
            {
                // Common cause with Gmail: need an App Password or OAuth2.
                _logger.LogError(smtpEx, "SMTP send failed. If using Gmail, ensure you are using an App Password and that the account allows SMTP access. See: https://support.google.com/accounts/answer/185833");
                throw;
            }
            finally
            {
                // Dispose attachment if any
                //attachment?.Dispose();
            }
        }




        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("London Estate", "Hello@LondonEstateGroup.co.uk"));
                message.To.Add(new MailboxAddress("Mohsen Alizadeh", recipientEmail));
                message.Subject = subject;

                message.Body = new TextPart("html") { Text = body };

                using (var client = new SmtpClient())
                {
                    // Connect to IONOS using Port 587
                    await client.ConnectAsync("smtp.ionos.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                    // Authenticate with your IONOS credentials
                    await client.AuthenticateAsync("hello@londonestategroup.co.uk", "Sina&nima2!");

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                throw ex;
            }
        }
    }
}