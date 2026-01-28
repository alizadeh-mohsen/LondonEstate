using LondonEstate.Models;

namespace LondonEstate.Services
{
    public interface IEstimateRequestService
    {
        public Task<List<string>> SubmitEstimateRequest(Customer customer, Property property, IEnumerable<IFormFile>? imageFiles);
    }
}
