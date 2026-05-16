using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LondonEstate.Data;
using LondonEstate.Models;

namespace LondonEstate.Pages.Admin.Bills
{
    public class DeleteModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public DeleteModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Bill Bill { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill.FirstOrDefaultAsync(m => m.Id == id);

            if (bill == null)
            {
                return NotFound();
            }
            else
            {
                Bill = bill;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill.FindAsync(id);
            if (bill != null)
            {
                Bill = bill;
                _context.Bill.Remove(Bill);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
