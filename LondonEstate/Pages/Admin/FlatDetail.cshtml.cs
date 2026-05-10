using LondonEstate.Data;
using LondonEstate.Models;
using LondonEstate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{

    [Authorize]
    public class FlatDetailModel : PageModel

    {
        private readonly ApplicationDbContext _context;
        private readonly ILogError _logError;

        public FlatDetailModel(ApplicationDbContext context, ILogError logError)
        {
            _context = context;
            _logError = logError;

        }

        public IList<Flat> Flat { get; set; } = default!;

        public async Task OnGetAsync()
        {
            try
            {
                var query = from f in _context.Flat
                            orderby f.Name
                            select f;
                Flat = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                await _logError.LogErrorToDb(ex, "Flats ");
                Flat = new List<Flat>();
            }
        }
    }
}

