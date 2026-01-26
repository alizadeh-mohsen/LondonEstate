using LondonEstate.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace LondonEstate.Services
{
    public interface IEstimateRequestService
    {
        // updated to accept multiple files
        public Task SubmitEstimateRequest(Customer customer, Property property, IEnumerable<IFormFile>? imageFiles);
    }
}
