using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{
    public class PropertiesModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public PropertiesModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Flat> Flat { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Flat = await _context.Flat.ToListAsync();
        }
    }
}
