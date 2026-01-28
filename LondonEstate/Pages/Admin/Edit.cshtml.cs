using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin
{
    [Authorize]
    public class EditModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
