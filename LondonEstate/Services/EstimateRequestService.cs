using LondonEstate.Data;
using LondonEstate.Models;

namespace LondonEstate.Services
{
    public class EstimateRequestService : IEstimateRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public EstimateRequestService(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task SubmitEstimateRequest(Customer customer, Property property, IFormFile? ImageFile)
        {
            _db.Customer.Add(customer);
            _db.Property.Add(property);

            string? savedFilePath = null;

            // handle image
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var fileExt = Path.GetExtension(ImageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExt}";
                var filePath = Path.Combine(uploads, fileName);

                await using (var stream = File.Create(filePath))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                savedFilePath = filePath;

                var imageRecord = new PropertyImage
                {
                    Property = property,
                    PropertyId = property.Id,
                    PicturePath = $"/uploads/{fileName}"
                };

                _db.PropertyImage.Add(imageRecord);
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (savedFilePath != null && File.Exists(savedFilePath))
                {
                    File.Delete(savedFilePath);
                }
                throw; // rethrow the exception after cleanup
            }
        }
    }
}
