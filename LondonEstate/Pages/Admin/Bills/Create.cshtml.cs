using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LondonEstate.Pages.Admin.Bills
{
    public class CreateModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public CreateModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["BillTypeId"] = new SelectList(_context.BillType.OrderBy(bt => bt.Name), "Id", "Name");
            ViewData["FlatId"] = new SelectList(_context.Flat.OrderBy(f => f.Name), "Id", "Name");
            ViewData["VendorId"] = new SelectList(_context.Vendor.OrderBy(v => v.Name), "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Bill Bill { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns when redisplaying the page after validation errors
                ViewData["BillTypeId"] = new SelectList(_context.BillType.OrderBy(bt => bt.Name), "Id", "Name");
                ViewData["FlatId"] = new SelectList(_context.Flat.OrderBy(f => f.Name), "Id", "Name");
                ViewData["VendorId"] = new SelectList(_context.Vendor.OrderBy(v => v.Name), "Id", "Name");

                return Page();
            }

            _context.Bill.Add(Bill);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
