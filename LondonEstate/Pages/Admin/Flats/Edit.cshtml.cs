using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Flats
{
    [Authorize]
    public class EditModel(IFlatService flatService) : PageModel
    {

        //private readonly IWebHostEnvironment _webHostEnvironment;

        //  public EditModel(
        //Data.ApplicationDbContext context,
        //IWebHostEnvironment webHostEnvironment,
        //IOptions<UploadSettings> uploadSettingsOptions)
        //  {
        //      _context = context;
        //      _webHostEnvironment = webHostEnvironment;
        //  }


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

        public async Task<IActionResult> OnPostAsync(IFormFile? imageUpload)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Load existing flat from DB
            var flatFromDb = await flatService.GetFlatAsync(Flat.Id);
            if (flatFromDb == null)
                return NotFound();

            // Update simple fields
            flatFromDb.Name = Flat.Name;
            flatFromDb.OnlineName = Flat.OnlineName;
            flatFromDb.Address = Flat.Address;
            flatFromDb.FlatUrl = Flat.FlatUrl;
            flatFromDb.Wifi = Flat.Wifi;
            flatFromDb.CheckinInstruction = Flat.CheckinInstruction;
            flatFromDb.Open = Flat.Open;

            // Handle image upload
            //if (imageUpload != null && imageUpload.Length > 0)
            //{
            //    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");

            //    if (!Directory.Exists(uploadsFolder))
            //        Directory.CreateDirectory(uploadsFolder);

            //    string fileExtension = Path.GetExtension(imageUpload.FileName);
            //    string newFileName = $"{Guid.NewGuid()}{fileExtension}";
            //    string filePath = Path.Combine(uploadsFolder, newFileName);

            //    using (var fileStream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await imageUpload.CopyToAsync(fileStream);
            //    }

            //    // Save relative path to DB
            //    flatFromDb.Image = $"/Images/{newFileName}";
            //}
            await flatService.UpdateFlat(flatFromDb);


            return RedirectToPage("./Index");
        }
    }
}
