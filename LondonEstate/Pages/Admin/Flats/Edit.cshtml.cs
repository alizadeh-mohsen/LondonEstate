using LondonEstate.Models;
using LondonEstate.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LondonEstate.Pages.Admin.Flats
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UploadSettings _uploadSettings;

        public EditModel(
      Data.ApplicationDbContext context,
      IWebHostEnvironment webHostEnvironment,
      IOptions<UploadSettings> uploadSettingsOptions)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _uploadSettings = uploadSettingsOptions.Value;
        }


        [BindProperty]
        public Flat Flat { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var flat = await _context.Flat.FirstOrDefaultAsync(m => m.Id == id);
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
            var flatFromDb = await _context.Flat.FirstOrDefaultAsync(f => f.Id == Flat.Id);
            if (flatFromDb == null)
                return NotFound();

            // Update simple fields
            flatFromDb.Name = Flat.Name;
            flatFromDb.OnlineName = Flat.OnlineName;
            flatFromDb.Address = Flat.Address;
            flatFromDb.FlatUrl = Flat.FlatUrl;
            flatFromDb.Wifi = Flat.Wifi;
            flatFromDb.CheckinInstruction = Flat.CheckinInstruction;

            // Handle image upload
            if (imageUpload != null && imageUpload.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileExtension = Path.GetExtension(imageUpload.FileName);
                string newFileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, newFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUpload.CopyToAsync(fileStream);
                }

                // Save relative path to DB
                flatFromDb.Image = $"/Images/{newFileName}";
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
