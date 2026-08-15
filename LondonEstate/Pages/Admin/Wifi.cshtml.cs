using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin
{
    [Authorize]
    public class WifiModel(IFlatService flatService) : PageModel
    {
        public IList<FlatDto> Flats { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Flats = await flatService.GetAllFlatsInfoAsync();
        }
    }
}
