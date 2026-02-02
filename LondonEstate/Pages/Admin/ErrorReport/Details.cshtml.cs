using LondonEstate.Data;
using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.ErrorReport
{
    //[Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ErrorLog ErrorLog { get; set; }

        public async Task OnGetAsync(int id)
        {
            ErrorLog = await _context.ErrorLogs.FindAsync(id);
        }
    }
}
