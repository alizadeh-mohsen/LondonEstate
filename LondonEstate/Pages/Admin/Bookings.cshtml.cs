using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace LondonEstate.Pages.Admin
{
    //[Authorize]
    public class BookingsModel(IFlatService _flatService) : PageModel
    {

        public IList<BookingDto> Flats { get; set; } = default!;
        public IList<BookingDto> EmptyFlats { get; set; } = default!;
        //public IList<BookingDto> EmptyTomorrowFlats { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Flats = await _flatService.GetBookingsAsync();
            var cutoff = DateTime.Today.AddHours(11);
            EmptyFlats = [.. Flats.Where(f => f.CheckOut < cutoff).OrderBy(f => f.Name).Select(f => new BookingDto
            {
                Id = f.Id,
                Name = f.Name
            })];
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
                            var location = worksheet.Cells[row, 2]?.Value?.ToString()?.Trim();
                            var bookerName = worksheet.Cells[row, 3]?.Value?.ToString()?.Trim();
                            var arrivalStr = worksheet.Cells[row, 4]?.Value?.ToString()?.Trim();
                            var departureStr = worksheet.Cells[row, 5]?.Value?.ToString()?.Trim();
                            var bookingNumber = worksheet.Cells[row, 6]?.Value?.ToString()?.Trim();
                            var phone = worksheet.Cells[row, 7]?.Value?.ToString()?.Trim();

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
                                    Departure = departure,
                                    BookingNumber = bookingNumber,
                                    PhoneNumber = phone,
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

                var result = await _flatService.ImportBookingsAsync(booking);
                if (result > 0)
                    updatedCount++;
            }

            return updatedCount;
        }


        public async Task<IActionResult> OnPostBackupAsync()
        {
            try
            {
                await _flatService.BackupAsync();
                SuccessMessage = "Successfully backed up all flats.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while backing up flats: {ex.Message}";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRecoverAsync()
        {
            try
            {
                await _flatService.RestoreAsync();
                SuccessMessage = "Successfully recovered all flats from backup.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while recovering from backup: {ex.Message}";
            }

            return RedirectToPage();
        }

    }
}