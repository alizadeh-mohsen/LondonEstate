using LondonEstate.Data;
using LondonEstate.Models;
using LondonEstate.Utils.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.RentEstimate
{
    public class EstimationRequestModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EstimationRequestModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Customer> Customers { get; set; } = default!;
        public MetaData MetaData { get; set; } = default!;

        public async Task OnGetAsync([FromQuery] QueryParams queryParams)
        {
            var query = _context.Customer.AsNoTracking().AsQueryable();
            query = query.Where(c => c.Properties.Any(c => c.EstimatedPrice == null));
            query = query.OrderBy(c => c.CreatedAt);

            var customers = await PagedList<Customer>.ToPagedList(query, queryParams.PageNumber, queryParams.PageSize);
            MetaData = customers.MetaData;
            Customers = customers;
        }

    }
}