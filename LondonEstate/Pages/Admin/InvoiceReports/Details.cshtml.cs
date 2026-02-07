using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.InvoiceReports
{
    public class DetailsModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public DetailsModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.Invoice Invoice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice.FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }
            else
            {
                Invoice = invoice;
            }
            return Page();
        }
    }
}
