using LondonEstate.Data;
using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{
    public class FlatDetailModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FlatDetailModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Flat Flat { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flat = await _context.Flat.FirstOrDefaultAsync(m => m.Id == id);
            if (flat == null)
            {
                return NotFound();
            }
            else
            {
                Flat = flat;
            }
            return Page();
        }
    }
}
