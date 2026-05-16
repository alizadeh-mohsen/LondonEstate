using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.BillTypes
{
    public class DeleteModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public DeleteModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BillType BillType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billtype = await _context.BillType.FirstOrDefaultAsync(m => m.Id == id);

            if (billtype == null)
            {
                return NotFound();
            }
            else
            {
                BillType = billtype;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billtype = await _context.BillType.FindAsync(id);
            if (billtype != null)
            {
                BillType = billtype;
                _context.BillType.Remove(BillType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
