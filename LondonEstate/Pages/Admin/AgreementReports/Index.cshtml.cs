using LondonEstate.Data;
using LondonEstate.Utils.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.AgreementReports
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Agreement> Agreements { get; set; } = default!;
        public MetaData MetaData { get; set; } = default!;

        public async Task OnGetAsync([FromQuery] QueryParams queryParams, string? search, DateTime? date)
        {
            var query = _context.Agreement.AsQueryable().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(c =>
                    (c.GuestName != null && c.GuestName.ToLower().Contains(lowerSearch))
                );
            }

            if (date.HasValue)
            {
                query = query.Where(c => c.Date.Date == date.Value.Date);
            }
            var agreements = await PagedList<Models.Agreement>.ToPagedList(query, queryParams.PageNumber, queryParams.PageSize);
            MetaData = agreements.MetaData;
            Agreements = agreements;
        }
    }
}

