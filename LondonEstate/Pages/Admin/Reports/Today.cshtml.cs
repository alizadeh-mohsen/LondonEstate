using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.Reports
{
    public class TodayModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;

        public TodayModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Flat> TodayCheckIns { get; set; } = new List<Flat>();
        public IList<Flat> TodayCheckOuts { get; set; } = new List<Flat>();
        public DateTime CurrentDate { get; set; }

        public async Task OnGetAsync()
        {
            CurrentDate = DateTime.Today;

            // Get all flats checking in today
            TodayCheckIns = await _context.Flat
                .Where(f => f.CheckIn.HasValue && f.CheckIn.Value.Date == CurrentDate && f.Open == true)
                .OrderBy(f => f.Name)
                .ToListAsync();

            // Get all flats checking out today
            TodayCheckOuts = await _context.Flat
                .Where(f => f.CheckOut.HasValue && f.CheckOut.Value.Date == CurrentDate && f.Open == true)
                .OrderBy(f => f.Name)
                .ToListAsync();
        }
    }
}
