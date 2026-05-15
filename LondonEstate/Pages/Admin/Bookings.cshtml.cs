using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace LondonEstate.Pages.Admin
{
    [Authorize]
    public class BookingsModel : PageModel
    {
        private readonly Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookingsModel(Data.ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public IList<Flat> Flat { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var query = from f in _context.Flat
                        orderby f.Name
                        select f;

            Flat = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostUploadAsync(IFormFile? excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ErrorMessage = "Please select an Excel file to upload.";
                return RedirectToPage();
            }

            // Validate file extension
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(excelFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ErrorMessage = "Only Excel files (.xlsx, .xls) are allowed.";
                return RedirectToPage();
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            ErrorMessage = "The Excel file does not contain any worksheets.";
                            return RedirectToPage();
                        }

                        int rowCount = worksheet.Dimension?.Rows ?? 0;
                        if (rowCount < 2)
                        {
                            ErrorMessage = "The Excel file must contain headers and at least one data row.";
                            return RedirectToPage();
                        }

                        // Parse Excel data
                        var bookingData = new List<BookingImportDto>();
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var propertyName = worksheet.Cells[row, 1]?.Value?.ToString()?.Trim();
                            var bookerName = worksheet.Cells[row, 3]?.Value?.ToString()?.Trim();
                            var arrivalStr = worksheet.Cells[row, 5]?.Value?.ToString()?.Trim();
                            var departureStr = worksheet.Cells[row, 6]?.Value?.ToString()?.Trim();

                            if (string.IsNullOrEmpty(propertyName))
                                continue;

                            if (DateTime.TryParse(arrivalStr, out var arrival) &&
                                DateTime.TryParse(departureStr, out var departure))
                            {
                                bookingData.Add(new BookingImportDto
                                {
                                    PropertyName = propertyName,
                                    BookerName = bookerName,
                                    Arrival = arrival,
                                    Departure = departure
                                });
                            }
                        }

                        if (bookingData.Count == 0)
                        {
                            ErrorMessage = "No valid booking data found in the Excel file.";
                            return RedirectToPage();
                        }

                        // Update database
                        int updatedCount = await UpdateFlatsFromImportAsync(bookingData);
                        SuccessMessage = $"Successfully updated {updatedCount} booking(s).";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while processing the file: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task<int> UpdateFlatsFromImportAsync(List<BookingImportDto> bookingData)
        {
            int updatedCount = 0;

            foreach (var booking in bookingData)
            {
                var flat = await _context.Flat
                    .FirstOrDefaultAsync(f => f.OnlineName != null && f.OnlineName.ToLower() == booking.PropertyName.ToLower());

                if (flat != null)
                {
                    flat.GuestName = booking.BookerName;
                    flat.CheckIn = booking.Arrival;
                    flat.CheckOut = booking.Departure;
                    flat.Empty = false;

                    _context.Flat.Update(flat);
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return updatedCount;
        }

        private class BookingImportDto
        {
            public string PropertyName { get; set; } = string.Empty;
            public string? BookerName { get; set; }
            public DateTime Arrival { get; set; }
            public DateTime Departure { get; set; }
        }
    }
}
