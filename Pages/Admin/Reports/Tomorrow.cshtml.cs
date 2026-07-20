using System.Text;
using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;

namespace LondonEstate.Pages.Admin.Reports
{
    [Authorize]
    public class TomorrowModel : PageModel
    {
        public void OnGet()
        {
        }

        // POST handler used by the page JavaScript to parse an uploaded Excel file
        public async Task<IActionResult> OnPostUploadAsync(IFormFile? excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return new JsonResult(new { error = "Please select an Excel file to upload." });
            }

            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var ext = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                return new JsonResult(new { error = "Only Excel files (.xlsx, .xls) are allowed." });
            }

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return new JsonResult(new { error = "The Excel file does not contain any worksheets." });

                var rowCount = worksheet.Dimension?.Rows ?? 0;
                var colCount = worksheet.Dimension?.Columns ?? 0;
                if (rowCount < 2)
                    return new JsonResult(new { error = "The Excel file must contain headers and at least one data row." });

                // Build header map from first row
                var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int c = 1; c <= colCount; c++)
                {
                    var header = worksheet.Cells[1, c]?.Value?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(header))
                        headerMap[NormalizeHeader(header)] = c;
                }

                bool TryGetHeader(string name, out int col, params string[] alternatives)
                {
                    col = -1;
                    var key = NormalizeHeader(name);
                    if (headerMap.TryGetValue(key, out col)) return true;
                    if (alternatives != null)
                    {
                        foreach (var alt in alternatives)
                        {
                            var aKey = NormalizeHeader(alt);
                            if (headerMap.TryGetValue(aKey, out col)) return true;
                        }
                    }
                    return false;
                }

                // Resolve columns (with fallbacks to common names or to fixed columns)
                if (!TryGetHeader("Property Name", out var colProperty, "Property", "Name", "PropertyName"))
                {
                    colProperty = 1; // fallback to column A
                }
                TryGetHeader("Booking Number", out var colBooking, "Booking No", "Reservation No", "BookingNumber");
                TryGetHeader("Phone", out var colPhone, "Phone Number", "Guest Phone", "Telephone");

                var rows = new List<object>();
                for (int r = 2; r <= rowCount; r++)
                {
                    var propertyName = colProperty > 0 ? worksheet.Cells[r, colProperty]?.Value?.ToString()?.Trim() : null;
                    var bookingNumber = colBooking > 0 ? worksheet.Cells[r, colBooking]?.Value?.ToString()?.Trim() : null;
                    var phone = colPhone > 0 ? worksheet.Cells[r, colPhone]?.Value?.ToString()?.Trim() : null;

                    if (string.IsNullOrWhiteSpace(propertyName))
                        continue;

                    rows.Add(new
                    {
                        PropertyName = propertyName,
                        BookingNumber = bookingNumber ?? string.Empty,
                        Phone = phone ?? string.Empty
                    });
                }

                if (rows.Count == 0)
                    return new JsonResult(new { error = "No valid booking data found in the Excel file.", rows = Array.Empty<object>() });

                return new JsonResult(rows);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"An error occurred while processing the file: {ex.Message}" });
            }
        }

        private static string NormalizeHeader(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return string.Empty;
            var sb = new StringBuilder(header.Length);
            foreach (var ch in header.ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(ch))
                    sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}