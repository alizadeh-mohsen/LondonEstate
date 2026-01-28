using LondonEstate.Data;
using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.ErrorReport
{

    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ErrorLog ErrorLog { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ErrorLog = await _context.ErrorLogs.FindAsync(id);

            if (ErrorLog == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var log = await _context.ErrorLogs.FindAsync(ErrorLog.Id);

            if (log != null)
            {
                _context.ErrorLogs.Remove(log);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
