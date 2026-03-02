using LondonEstate.Data;
using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.Rents
{
    public class SettleModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SettleModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rent Rent { get; set; } = default!;

        public RentHistory? LastSettlement { get; set; }

        // Added: list of history entries for the UI
        public List<RentHistory> RentHistories { get; set; } = new();

        [TempData]
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Rent = await _context.Rent.FirstOrDefaultAsync(r => r.Id == id);

            if (Rent == null)
            {
                return NotFound();
            }

            LastSettlement = await _context.RentHistory
                .Where(h => h.RentId == id)
                .OrderByDescending(h => h.PaidDate)
                .FirstOrDefaultAsync();

            // Populate the list of settlements (new)
            RentHistories = await _context.RentHistory
                .Where(h => h.RentId == id)
                .OrderByDescending(h => h.PaidDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            // Validate rent exists
            var rent = await _context.Rent.FirstOrDefaultAsync(r => r.Id == id);
            if (rent == null)
            {
                return NotFound();
            }

            var history = new RentHistory
            {
                RentId = id,
                PaidDate = DateTime.UtcNow
            };

            _context.RentHistory.Add(history);
            await _context.SaveChangesAsync();

            Message = "Rent settled successfully.";

            // Redirect to GET to show updated last settlement and list
            return RedirectToPage("./Settle", new { id });
        }
    }
}