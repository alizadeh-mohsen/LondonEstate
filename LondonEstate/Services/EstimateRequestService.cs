using LondonEstate.Data;
using LondonEstate.Models;
using Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using LondonEstate.Settings;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace LondonEstate.Services
{
    public class EstimateRequestService : IEstimateRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly UploadSettings _uploadSettings;

        public EstimateRequestService(ApplicationDbContext db, IWebHostEnvironment env, IOptions<UploadSettings> uploadSettings)
        {
            _db = db;
            _env = env;
            _uploadSettings = uploadSettings?.Value ?? new UploadSettings();
        }

        // now uses configuration-driven limits and validation
        public async Task SubmitEstimateRequest(Customer customer, Property property, IEnumerable<IFormFile>? imageFiles)
        {
            _db.Customer.Add(customer);
            _db.Property.Add(property);

            if (imageFiles != null)
            {
                var files = imageFiles.Where(f => f != null).ToArray();

                if (files.Length > _uploadSettings.MaxFiles)
                {
                    throw new ArgumentException($"Too many files. Maximum allowed is {_uploadSettings.MaxFiles}.");
                }

                var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", _uploadSettings.UploadFolder);
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var allowed = new HashSet<string>(_uploadSettings.AllowedExtensions.Select(e => e.ToLowerInvariant()));

                long maxBytes = (long)_uploadSettings.MaxFileSizeMB * 1024 * 1024;

                foreach (var file in files)
                {
                    if (file.Length == 0)
                    {
                        throw new ArgumentException("One of the files is empty.");
                    }

                    if (file.Length > maxBytes)
                    {
                        throw new ArgumentException($"File '{file.FileName}' exceeds the maximum allowed size of {_uploadSettings.MaxFileSizeMB} MB.");
                    }

                    var fileExt = Path.GetExtension(file.FileName);
                    if (string.IsNullOrWhiteSpace(fileExt) || !allowed.Contains(fileExt.ToLowerInvariant()))
                    {
                        throw new ArgumentException($"File '{file.FileName}' has an invalid file type. Allowed: {string.Join(", ", _uploadSettings.AllowedExtensions)}.");
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
                        PicturePath = $"/{_uploadSettings.UploadFolder}/{fileName}"
                    };

                    _db.PropertyImage.Add(imageRecord);
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
