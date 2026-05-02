using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LondonEstate.Data;
using LondonEstate.Models;

namespace LondonEstate.Pages.Admin.Flats
{
    public class DetailsModel : PageModel
    {
        private readonly LondonEstate.Data.ApplicationDbContext _context;

        public DetailsModel(LondonEstate.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Flat Flat { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flat = await _context.Flat.FirstOrDefaultAsync(m => m.Id == id);
            if (flat == null)
            {
                return NotFound();
            }
            else
            {
                Flat = flat;
            }
            return Page();
        }
    }
}
