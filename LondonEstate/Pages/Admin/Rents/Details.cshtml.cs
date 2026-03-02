using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.Rents
{
    public class DetailsModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public DetailsModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Rent Rent { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rent = await _context.Rent.FirstOrDefaultAsync(m => m.Id == id);
            if (rent == null)
            {
                return NotFound();
            }
            else
            {
                Rent = rent;
            }
            return Page();
        }
    }
}
