using LondonEstate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

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
        public IList<Flat> EmptyFlats { get; set; } = default!;
        public IList<Flat> EmptyTomorrowFlats { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var query = from f in _context.Flat
                        where f.Open == true
                        orderby f.Name
                        select new Flat
                        {
                            Id = f.Id,
                            Name = f.Name,
                            OnlineName = f.OnlineName,
                            CheckIn = f.CheckIn,
                            CheckOut = f.CheckOut,
                            GuestName = f.GuestName
                        };

            Flat = await query.ToListAsync();
            var cutoff = DateTime.Today.AddHours(11);


            EmptyFlats = Flat.Where(f => f.CheckOut < cutoff).OrderBy(f => f.Name).ToList();


            EmptyTomorrowFlats = Flat.Where(f => f.CheckOut >= cutoff && f.CheckOut < cutoff.AddDays(1)).OrderBy(f => f.Name).ToList();
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

            //backup existing flats before updating
            await BackupFlats();

            foreach (var booking in bookingData)
            {
                var flat = await _context.Flat
                    .FirstOrDefaultAsync(f => f.OnlineName != null && f.OnlineName.ToLower() == booking.PropertyName.ToLower());

                if (flat != null)
                {
                    flat.GuestName = booking.BookerName;
                    flat.CheckIn = booking.Arrival;
                    flat.CheckOut = booking.Departure;
                    flat.Open = true;

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

        public async Task<IActionResult> OnPostBackupAsync()
        {
            try
            {
                var existingFlats = await _context.Flat.ToListAsync();
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE FlatBackup");

                foreach (var flat in existingFlats)
                {
                    var flatBackup = new FlatBackup
                    {
                        Id = flat.Id,
                        Name = flat.Name,
                        OnlineName = flat.OnlineName,
                        GuestName = flat.GuestName,
                        CheckIn = flat.CheckIn,
                        CheckOut = flat.CheckOut
                    };
                    _context.FlatBackup.Add(flatBackup);
                }

                await _context.SaveChangesAsync();
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
                var backupFlats = await _context.FlatBackup.ToListAsync();

                if (backupFlats.Count == 0)
                {
                    ErrorMessage = "No backup data found.";
                    return RedirectToPage();
                }

                // Restore all flats from backup
                foreach (var backup in backupFlats)
                {
                    var flat = await _context.Flat.FirstOrDefaultAsync(f => f.Id == backup.Id);

                    if (flat != null)
                    {
                        flat.GuestName = backup.GuestName;
                        flat.CheckIn = backup.CheckIn;
                        flat.CheckOut = backup.CheckOut;

                        _context.Flat.Update(flat);
                    }
                }

                await _context.SaveChangesAsync();
                SuccessMessage = $"Successfully recovered {backupFlats.Count} booking(s) from backup.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while recovering from backup: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostGenerateReportAsync()
        {
            try
            {
                var flats = await _context.Flat
                    .Where(f => f.Open == true)
                    .OrderBy(f => f.Name)
                    .ToListAsync();

                if (flats.Count == 0)
                {
                    ErrorMessage = "No bookings available to generate a report.";
                    return RedirectToPage();
                }

                var pdfBytes = GenerateCheckoutReport(flats);
                var fileName = $"Checkout_Report_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while generating the report: {ex.Message}";
                return RedirectToPage();
            }
        }

        private byte[] GenerateCheckoutReport(IList<Flat> flats)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    page.Header().Text("Check-out Report")
                        .FontSize(24)
                        .Bold()
                        .FontColor(Colors.Blue.Darken4);

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        // Report Header
                        column.Item().Text($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken2);

                        column.Item().Container().Background(Colors.Blue.Lighten4).Padding(5).Text($"Total Properties: {flats.Count}")
                            .FontSize(12)
                            .Bold();

                        // Table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2f);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1f);
                            });

                            // Header Row
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Darken4).Padding(5).Text("Property Name")
                                    .Bold()
                                    .FontColor(Colors.White)
                                    .FontSize(11);

                                header.Cell().Background(Colors.Blue.Darken4).Padding(5).Text("Check-in Date")
                                    .Bold()
                                    .FontColor(Colors.White)
                                    .FontSize(11);

                                header.Cell().Background(Colors.Blue.Darken4).Padding(5).Text("Check-out Date")
                                    .Bold()
                                    .FontColor(Colors.White)
                                    .FontSize(11);

                                header.Cell().Background(Colors.Blue.Darken4).Padding(5).Text("Days Left")
                                    .Bold()
                                    .FontColor(Colors.White)
                                    .FontSize(11);
                            });

                            // Data Rows
                            foreach (var flat in flats)
                            {
                                var daysLeft = flat.CheckOut.HasValue
                                    ? (flat.CheckOut.Value.Date - DateTime.Today).Days
                                    : -1;

                                var backgroundColor = /*daysLeft < 1 ? Colors.Red.Lighten3 :*/
                                                     daysLeft == 1 ? Colors.Red.Lighten4 :
                                                     Colors.White;

                                table.Cell().Background(backgroundColor).Padding(5).Text(flat.Name ?? "N/A")
                                    .FontSize(10);

                                table.Cell().Background(backgroundColor).Padding(5).Text(flat.CheckIn?.ToString("dd/MM/yyyy") ?? "N/A")
                                    .FontSize(10)
                                .FontColor(daysLeft < 1 ? Colors.White : Colors.Black);

                                table.Cell().Background(backgroundColor).Padding(5).Text(flat.CheckOut?.ToString("dd/MM/yyyy") ?? "N/A")
                                    .FontSize(10)
                                .FontColor(daysLeft < 1 ? Colors.White : Colors.Black);

                                table.Cell().Background(backgroundColor).Padding(5).Text(daysLeft >= 0 ? daysLeft.ToString() : "Empty")
                                    .Bold()
                                    .FontSize(10)
                                .FontColor(daysLeft < 1 ? Colors.White : Colors.Black);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private async Task BackupFlats()
        {

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