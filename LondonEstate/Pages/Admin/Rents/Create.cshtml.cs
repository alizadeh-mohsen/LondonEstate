using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Rents
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
            return Page();
        }

        [BindProperty]
        public Rent Rent { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Rent.Add(Rent);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
