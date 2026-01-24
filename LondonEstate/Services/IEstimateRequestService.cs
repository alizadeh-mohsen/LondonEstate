using LondonEstate.Models;

namespace LondonEstate.Services
{
    public interface IEstimateRequestService
    {
        public Task SubmitEstimateRequest(Customer customer, Property property, IFormFile? ImageFile);
    }
}
