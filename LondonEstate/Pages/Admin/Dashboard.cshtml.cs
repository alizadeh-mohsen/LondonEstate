using LondonEstate.Data;
using LondonEstate.Models;
using LondonEstate.Utils.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Customer> Customers { get; set; } = default!;
        public MetaData MetaData { get; set; } = default!;

        public async Task OnGetAsync([FromQuery] QueryParams queryParams)
        {
            var query = _context.Customer.AsNoTracking().AsQueryable();
            //todo: show only customers who have properties with EstimateStatus.Received
            //todo: add search, filter, orderby functionality later

            query = query
                .Where(c => c.Properties.Any(p => p.EstimateStatus == Utils.Enums.EstimateStatus.Received))
                .OrderBy(c => c.CreatedAt);

            var customers = await PagedList<Customer>.ToPagedList(query, queryParams.PageNumber, queryParams.PageSize);
            MetaData = customers.MetaData;
            Customers = customers;
        }
    }
}