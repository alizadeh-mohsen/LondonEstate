using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.BillTypes
{
    public class IndexModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public IndexModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<BillType> BillType { get; set; } = default!;

        public async Task OnGetAsync()
        {
            BillType = await _context.BillType.ToListAsync();
        }
    }
}
