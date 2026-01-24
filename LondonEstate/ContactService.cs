namespace LondonEstate
{
    public class ContactService
    {
        private readonly IEmailSender _emailSender;

        public ContactService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendContactEmail()
        {
            await _emailSender.SendAsync(
                "kelarens@gmail.com",
                "IONOS",
                "<p>New message received</p>"
            );
        }
    }

}
