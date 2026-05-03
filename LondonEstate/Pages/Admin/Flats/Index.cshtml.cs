using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.Flats
{
    public class IndexModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public IndexModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Flat> Flat { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Flat = await _context.Flat.OrderBy(f => f.Name).ToListAsync();
        }
    }
}
