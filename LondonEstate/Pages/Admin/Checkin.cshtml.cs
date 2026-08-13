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

        // For the move operation
        [BindProperty]
        public Guid SourceFlatId { get; set; }

        [BindProperty]
        public Guid MoveToFlatId { get; set; }

        public List<FlatDto> Flats { get; set; } = new();

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
            Flats = await flatService.GetAllFlatsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Flats = await flatService.GetAllFlatsAsync();
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

        // Handler to move a booking from the current flat to another flat
        public async Task<IActionResult> OnPostMoveAsync()
        {
            // Ensure source and destination provided
            if (SourceFlatId == Guid.Empty || MoveToFlatId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Please select a target flat to move the booking to.");
                Flats = await flatService.GetAllFlatsAsync();
                // reload current booking for display
                Flat = await flatService.GetBookingAsync(SourceFlatId);
                return Page();
            }

            if (SourceFlatId == MoveToFlatId)
            {
                ModelState.AddModelError(string.Empty, "Cannot move booking to the same flat.");
                Flats = await flatService.GetAllFlatsAsync();
                Flat = await flatService.GetBookingAsync(SourceFlatId);
                return Page();
            }

            // Get source booking
            BookingDto sourceBooking;
            try
            {
                sourceBooking = await flatService.GetBookingAsync(SourceFlatId);
            }
            catch
            {
                return NotFound();
            }

            // Create a booking DTO for the target flat
            var targetBooking = new BookingDto
            {
                Id = MoveToFlatId,
                CheckIn = sourceBooking.CheckIn,
                CheckOut = sourceBooking.CheckOut,
                GuestName = sourceBooking.GuestName,
                GuestPhone = sourceBooking.GuestPhone,
                BookingNumber = sourceBooking.BookingNumber
            };

            await flatService.UpdateBookingAsync(targetBooking);

            // Optionally keep source booking as-is. If you want to clear source, implement here.

            return RedirectToPage("./bookings");
        }

        private async Task<bool> FlatExists(Guid id)
        {
            return await flatService.FlatExists(id);
        }
    }
}
