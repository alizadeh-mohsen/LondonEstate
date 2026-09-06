using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.reminders
{
    //[Authorize]
    public class IndexModel(IReminderService reminderService) : PageModel
    {
        public IList<ReminderDto> Reminder { get; set; } = default!;

        public async Task OnGetAsync()
        {
            try
            {
                Reminder = await reminderService.GetAllRemindersAsync();
            }
            catch (Exception ex)
            {
                Reminder = new List<ReminderDto>();
            }
        }
    }
}
