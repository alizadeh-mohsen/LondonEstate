using LondonEstate.Data;
using LondonEstate.Models;
using LondonEstate.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

namespace LondonEstate.Services
{
    public class EstimateRequestService : IEstimateRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogError _logError;
        private readonly UploadSettings _uploadSettings;

        public EstimateRequestService(ApplicationDbContext db, 
            IWebHostEnvironment env, 
            ILogError logError,
            IOptions<UploadSettings> uploadSettings)
        {
            _db = db;
            _env = env;
            _logError = logError;
            _uploadSettings = uploadSettings.Value;
        }

        // now uses configuration-driven limits and validation
        public async Task<List<string>> SubmitEstimateRequest(Customer customer, Property property, IEnumerable<IFormFile>? imageFiles)
        {
            List<string> errorList = new();
            try
            {
                var existingCustomer = _db.Customer.FirstOrDefault(c => c.Email == customer.Email);
                if (existingCustomer != null)
                {
                    property.CustomerId = existingCustomer.Id;
                }
                else
                {
                    _db.Customer.Add(customer);
                }

                _db.Property.Add(property);

                if (imageFiles != null && imageFiles.Any())
                {
                    var files = imageFiles.Where(f => f != null).ToArray();

                    if (files.Length > _uploadSettings.MaxFiles)
                        errorList.Add($"Too many files. Maximum allowed is {_uploadSettings.MaxFiles}.");

                    var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", _uploadSettings.EstimationUploadDirectory);
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    var allowed = new HashSet<string>(_uploadSettings.AllowedExtensions.Select(e => e.ToLowerInvariant()));

                    long maxBytes = (long)_uploadSettings.MaxFileSizeMB * 1024 * 1024;

                    foreach (var file in files)
                    {
                        if (file.Length == 0)
                            errorList.Add($"File '{file.FileName}' is empty.");

                        if (file.Length > maxBytes)
                            errorList.Add($"File '{file.FileName}' exceeds the maximum allowed size of {_uploadSettings.MaxFileSizeMB} MB.");

                        var fileExt = Path.GetExtension(file.FileName);
                        if (string.IsNullOrWhiteSpace(fileExt) || !allowed.Contains(fileExt.ToLowerInvariant()))
                        {
                            errorList.Add($"File '{file.FileName}' has an invalid file type. Allowed: {string.Join(", ", _uploadSettings.AllowedExtensions)}.");
                        }

                        var fileName = $"{Guid.NewGuid()}{fileExt}";
                        var filePath = Path.Combine(uploads, fileName);

                        await using (var stream = File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var imageRecord = new PropertyImage
                        {
                            Property = property,
                            PropertyId = property.Id,
                            PicturePath = $"/{_uploadSettings.EstimationUploadDirectory}/{fileName}"
                        };

                        _db.PropertyImage.Add(imageRecord);
                    }
                }

                await _db.SaveChangesAsync();
                return errorList;
            }
            catch (Exception ex)
            {
                var files = await _db.PropertyImage.Where(pi => pi.PropertyId.ToString() == property.Id.ToString()).ToListAsync();

                if (files.Count != 0)
                    foreach (var file in files)
                        File.Delete(_env.WebRootPath + file.PicturePath);

                Log.Error(ex, "<<<<< Error in SubmitEstimateRequest >>>>>>>");
                await _logError.LogErrorToDb( ex,"EstimateRequestService");
                return new List<string>() { "An error occurred while submitting your estimate request. Please try again later." };
            }
        }
    }
}
