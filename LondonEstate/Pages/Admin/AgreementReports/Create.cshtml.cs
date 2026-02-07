using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.AgreementReports
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
        public Models.Agreement Agreement { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Agreement.Add(Agreement);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
