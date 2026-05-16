using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin.Bills
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;
        private const int PageSize = 10;

        public IndexModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Bill> Bill { get; set; } = default!;

        public SelectList Flats { get; set; } = default!;
        public SelectList Vendors { get; set; } = default!;
        public SelectList BillTypes { get; set; } = default!;

        // Pagination metadata
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        // Filters (bound from query string)
        [BindProperty(SupportsGet = true)]
        public Guid? FilterFlatId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? FilterVendorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? FilterBillTypeId { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Bill
                .Include(b => b.BillType)
                .Include(b => b.Flat)
                .Include(b => b.Vendor)
                .OrderByDescending(b => b.PaidDate)
                .AsQueryable();

            // Apply server-side filters when provided
            if (FilterFlatId.HasValue)
            {
                query = query.Where(b => b.FlatId == FilterFlatId.Value);
            }

            if (FilterVendorId.HasValue)
            {
                query = query.Where(b => b.VendorId == FilterVendorId.Value);
            }

            if (FilterBillTypeId.HasValue)
            {
                query = query.Where(b => b.BillTypeId == FilterBillTypeId.Value);
            }

            TotalCount = await query.CountAsync();

            if (PageIndex < 1) PageIndex = 1;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            Bill = await query
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            Flats = new SelectList(await _context.Flat.OrderBy(f => f.Name).ToListAsync(), "Id", "Name", FilterFlatId);
            Vendors = new SelectList(await _context.Vendor.OrderBy(v => v.Name).ToListAsync(), "Id", "Name", FilterVendorId);
            BillTypes = new SelectList(await _context.BillType.OrderBy(bt => bt.Name).ToListAsync(), "Id", "Name", FilterBillTypeId);
        }
    }
}
