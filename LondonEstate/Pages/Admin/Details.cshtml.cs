using LondonEstate.Data;
using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.Customer Customer { get; set; } = default!;
        public IList<Property> Properties { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id.Value);

            if (customer == null)
            {
                return NotFound();
            }

            Customer = customer;

            // Fetch all properties for this customer
            Properties = await _context.Property
                .Include(p => p.Images)
                .Where(p => p.CustomerId == id.Value)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}