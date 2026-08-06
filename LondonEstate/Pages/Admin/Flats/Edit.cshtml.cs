using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Flats
{
    [Authorize]
    public class EditModel(IFlatService flatService) : PageModel
    {

        //private readonly IWebHostEnvironment _webHostEnvironment;

        //  public EditModel(
        //Data.ApplicationDbContext context,
        //IWebHostEnvironment webHostEnvironment,
        //IOptions<UploadSettings> uploadSettingsOptions)
        //  {
        //      _context = context;
        //      _webHostEnvironment = webHostEnvironment;
        //  }


        [BindProperty]
        public FlatDto Flat { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var flat = await flatService.GetFlatAsync(id);
            if (flat == null)
            {
                return NotFound();
            }
            Flat = flat;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updatedRows = await flatService.UpdateFlat(Flat);
            if (updatedRows == 0)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}
