using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.AgreementReports
{
    public class DetailsModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public DetailsModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.Agreement Agreement { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agreement = await _context.Agreement.FirstOrDefaultAsync(m => m.Id == id);
            if (agreement == null)
            {
                return NotFound();
            }
            else
            {
                Agreement = agreement;
            }
            return Page();
        }
    }
}
