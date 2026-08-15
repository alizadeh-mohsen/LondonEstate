using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Flats
{
    [Authorize]
    public class ListingsModel(IFlatService flatService) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid FlatId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FlatName { get; set; } = string.Empty;

        public List<ListingDto> Listings { get; set; } = new();

        [BindProperty]
        public ListingDto NewListing { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id, string name)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            FlatId = id;
            FlatName = name;
            Listings = await flatService.GetFlatListingsAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                Listings = await flatService.GetFlatListingsAsync(FlatId);
                return Page();
            }

            NewListing.FlatId = FlatId;
            await flatService.CreateListing(NewListing);
            return RedirectToPage(new { id = FlatId });
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            await flatService.DeleteListing(id);
            return RedirectToPage(new { id = FlatId });
        }
    }
}
