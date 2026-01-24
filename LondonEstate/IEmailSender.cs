using LondonEstate.Models;

namespace LondonEstate
{
    public interface IEmailSender
    {
        Task SendAsync(Customer customer, Property property, string? attachmentFilePath);
    }

}
