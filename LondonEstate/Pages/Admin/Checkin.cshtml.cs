using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{
    [Authorize]
    public class CheckinModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public CheckinModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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
            Flat = flat;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var flat = await _context.Flat.FirstOrDefaultAsync(m => m.Id == Flat.Id);
            if (flat == null)
            {
                return NotFound();
            }
            flat.CheckIn = Flat.CheckIn;
            flat.CheckOut = Flat.CheckOut;
            flat.ReservationUrl = Flat.ReservationUrl;
            flat.BookingNumber = Flat.BookingNumber;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlatExists(Flat.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./properties");
        }

        private bool FlatExists(Guid id)
        {
            return _context.Flat.Any(e => e.Id == id);
        }
    }
}
