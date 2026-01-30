using LondonEstate.Data;
using LondonEstate.Models;
using LondonEstate.Utils.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.Customers
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Customer> Customers { get;set; } = default!;
        public MetaData MetaData { get; set; } = default!;

        public async Task OnGetAsync([FromQuery] QueryParams queryParams,string? search)
        {
            var query = _context.Customer.AsQueryable().AsNoTracking();
            if(!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(lowerSearch)) ||
                    c.Email.ToLower().Contains(lowerSearch) ||
                    c.PhoneNumber.Contains(lowerSearch) ||
                    c.CountryCode.Contains(lowerSearch)
                );
            }
            var customers = await PagedList<Customer>.ToPagedList(query, queryParams.PageNumber, queryParams.PageSize);
            MetaData = customers.MetaData;
            Customers = customers;
        }
    }
}
