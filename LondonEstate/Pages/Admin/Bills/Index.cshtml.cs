using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LondonEstate.Data;
using LondonEstate.Models;

namespace LondonEstate.Pages.Admin.Bills
{
    public class IndexModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public IndexModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Bill> Bill { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Bill = await _context.Bill
                .Include(b => b.BillType)
                .Include(b => b.Flat)
                .Include(b => b.Vendor).ToListAsync();
        }
    }
}
