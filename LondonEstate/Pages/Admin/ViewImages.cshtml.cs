using LondonEstate.Data;
using LondonEstate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{
    public class ViewImagesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ViewImagesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Property Property { get; set; } = default!;
        public IList<PropertyImage> Images { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Property
                .Include(p => p.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id.Value);

            if (property == null)
            {
                return NotFound();
            }

            Property = property;

            // Fetch all images for this property
            Images = await _context.PropertyImage
                .Where(img => img.PropertyId == id.Value)
                .OrderBy(img => img.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}