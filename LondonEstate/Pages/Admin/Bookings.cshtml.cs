using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{
    [Authorize]
    public class BookingsModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public BookingsModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Flat> Flat { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var query = from f in _context.Flat
                        orderby f.Name
                        select f;

            Flat = await query.ToListAsync();
        }
    }
}
