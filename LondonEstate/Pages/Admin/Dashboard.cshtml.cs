using LondonEstate.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Customer> Customer { get; set; } = default!;

        // Pagination properties
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // Default page size; can be overridden via query string ?pageSize=20
        private const int DefaultPageSize = 10;

        // Accept optional query parameters pageIndex and pageSize
        public async Task OnGetAsync(int? pageIndex, int? pageSize)
        {
            PageIndex = Math.Max(pageIndex ?? 1, 1);
            PageSize = Math.Max(pageSize ?? DefaultPageSize, 1);

            // Base query - AsNoTracking for read-only paging
            var query = _context.Customer.AsNoTracking();

            // Ensure deterministic ordering for paging; use Id if present, otherwise Name
            // Replace 'Id' with the correct key property if different.
            try
            {
                query = query.OrderBy(c => EF.Property<object>(c, "Id"));
            }
            catch
            {
                query = query.OrderBy(c => c.Name);
            }

            TotalCount = await query.CountAsync();
            TotalPages = TotalCount == 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);

            // Clamp PageIndex to available range
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            Customer = await query
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}