using LondonEstate.Models;

namespace LondonEstate.Services
{
    public interface IEmailSender
    {
        Task SendEstimateRequestEmailAsync(Customer customer, Property property);
        //Task SendAsync(Customer customer, Property property, string? attachmentFilePath);
    }

}
