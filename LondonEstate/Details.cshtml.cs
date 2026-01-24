using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LondonEstate.Data;
using LondonEstate.Models;

namespace LondonEstate
{
    public class DetailsModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public DetailsModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            else
            {
                Customer = customer;
            }
            return Page();
        }
    }
}
