using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Flats
{
    [Authorize]
    public class IndexModel(IFlatService flatService) : PageModel
    {
        public IList<FlatDto> Flat { get; set; } = default!;

        public async Task OnGetAsync()
        {
            try
            {
                Flat = await flatService.GetAllFlatsAsync();
            }
            catch (Exception ex)
            {
                Flat = new List<FlatDto>();
            }
        }
    }
}
