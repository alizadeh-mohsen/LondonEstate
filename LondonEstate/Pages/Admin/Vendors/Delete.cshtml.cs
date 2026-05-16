using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.Vendors
{
    public class DeleteModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public DeleteModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Vendor Vendor { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _context.Vendor.FirstOrDefaultAsync(m => m.Id == id);

            if (vendor == null)
            {
                return NotFound();
            }
            else
            {
                Vendor = vendor;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _context.Vendor.FindAsync(id);
            if (vendor != null)
            {
                Vendor = vendor;
                _context.Vendor.Remove(Vendor);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
