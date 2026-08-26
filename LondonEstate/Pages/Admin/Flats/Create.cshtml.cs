using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Flats
{
    //[Authorize]
    public class CreateModel(IFlatService flatService) : PageModel
    {

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public FlatDto Flat { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await flatService.CreateFlat(Flat);
            return RedirectToPage("./Index");
        }
    }
}
