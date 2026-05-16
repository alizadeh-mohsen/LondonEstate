using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LondonEstate.Data;
using LondonEstate.Models;

namespace LondonEstate.Pages.Admin.Bills
{
    public class EditModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public EditModel(LondonEstate.Data.ApplicationDbContext context)
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

            var bill =  await _context.Bill.FirstOrDefaultAsync(m => m.Id == id);
            if (bill == null)
            {
                return NotFound();
            }
            Bill = bill;
           ViewData["BillTypeId"] = new SelectList(_context.BillType, "Id", "Id");
           ViewData["FlatId"] = new SelectList(_context.Flat, "Id", "Id");
           ViewData["VendorId"] = new SelectList(_context.Vendor, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Bill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillExists(Bill.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BillExists(Guid id)
        {
            return _context.Bill.Any(e => e.Id == id);
        }
    }
}
