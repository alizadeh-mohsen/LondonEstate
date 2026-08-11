using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LondonEstate.Pages.Admin
{
    [Authorize]
    public class CheckinModel(IFlatService flatService) : PageModel
    {

        [BindProperty]
        public BookingDto Flat { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var flat = await flatService.GetBookingAsync(id);
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

            try
            {
                await flatService.UpdateBookingAsync(Flat);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FlatExists(Flat.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./bookings");
        }

        private async Task<bool> FlatExists(Guid id)
        {
            return await flatService.FlatExists(id);
        }
    }
}
