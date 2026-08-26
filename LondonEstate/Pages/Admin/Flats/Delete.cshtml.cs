using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Flats
{
    //[Authorize]
    public class DeleteModel(IFlatService flatService) : PageModel
    {


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
            else
            {
                Flat = flat;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flat = await flatService.GetFlatAsync(id);
            if (flat != null)
            {
                Flat = flat;
                await flatService.DeleteFlat(id);
            }

            return RedirectToPage("./Index");
        }
    }
}
