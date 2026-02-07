using LondonEstate.Data;
using LondonEstate.Utils.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.InvoiceReports
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Invoice> Invoices { get; set; } = default!;
        public MetaData MetaData { get; set; } = default!;

        public async Task OnGetAsync([FromQuery] QueryParams queryParams, string? issuedTo, DateTime? date, string? property)

        {
            var query = _context.Invoice.AsQueryable().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(issuedTo))
            {
                string lowerSearch = issuedTo.ToLower();
                query = query.Where(c =>
                    (c.IssuedTo != null && c.IssuedTo.ToLower().Contains(lowerSearch))
                );
            }
            if (date.HasValue)
            {
                query = query.Where(c => c.Date.Date == date.Value.Date);
            }
            if (!string.IsNullOrWhiteSpace(property))
            {
                string lowerSearch = property.ToLower();
                query = query.Where(c =>
                    (c.Property != null && c.Property.ToLower().Contains(lowerSearch))
                );
            }
            var invoices = await PagedList<Models.Invoice>.ToPagedList(query, queryParams.PageNumber, queryParams.PageSize);
            MetaData = invoices.MetaData;
            Invoices = invoices;
        }
    }
}
