using LondonEstate.Data;
using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.ErrorReport
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ErrorLog> ErrorLogs { get; set; } = new List<ErrorLog>();

        public async Task OnGetAsync()
        {
            ErrorLogs = await _context.ErrorLogs
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }
    }

}
