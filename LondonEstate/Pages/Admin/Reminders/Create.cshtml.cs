using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Reminders
{
    //[Authorize]
    public class CreateModel(IReminderService reminderService) : PageModel
    {
        [BindProperty]
        public ReminderDto Reminder { get; set; } = default!;
        public IActionResult OnGet()
        {
            Reminder = new ReminderDto
            {
                Due = DateOnly.FromDateTime(DateTime.Today)
            };
            return Page();
        }



        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await reminderService.CreateReminderAsync(Reminder);
            return RedirectToPage("./Index");
        }
    }
}
