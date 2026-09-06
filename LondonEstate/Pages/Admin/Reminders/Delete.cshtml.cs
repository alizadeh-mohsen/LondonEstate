using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.reminders
{
    //[Authorize]
    public class DeleteModel(IReminderService reminderService) : PageModel
    {

        [BindProperty]
        public ReminderDto Reminder { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reminder = await reminderService.GetReminderAsync(id);

            if (reminder == null)
            {
                return NotFound();
            }
            else
            {
                Reminder = reminder;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reminder = await reminderService.GetReminderAsync(id);
            if (reminder != null)
            {
                Reminder = reminder;
                await reminderService.DeleteReminderAsync(id);
            }

            return RedirectToPage("./Index");
        }
    }
}
