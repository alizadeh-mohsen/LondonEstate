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
            var flat = await flatService.GetFlatAsync(Flat.Id);
            if (flat == null)
            {
                return NotFound();
            }
            flat.CheckIn = Flat.CheckIn;
            flat.CheckOut = Flat.CheckOut;
            flat.ReservationUrl = Flat.ReservationUrl;
            flat.BookingNumber = Flat.BookingNumber;
            flat.GuestPhone = Flat.GuestPhone;
            flat.GuestName = Flat.GuestName;
            flat.Open = true;
            try
            {
                await flatService.UpdateFlat(flat);
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
