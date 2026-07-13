using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;

namespace LondonEstate.Pages.Admin.Greetings
{
    public class GreetingData
    {
        public string PropertyName { get; set; }
        public string Location { get; set; }
        public string BookerName { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public string ReservationNo { get; set; }
        public string Phone { get; set; }
        public string WhatsAppMessage { get; set; }
    }

    [Authorize]
    public class IndexModel : PageModel
    {
        public List<GreetingData> Greetings { get; set; } = new();
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUploadAsync(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ErrorMessage = "Please select an Excel file to upload.";
                return Page();
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
                            return Page();
                        }

                        int rowCount = worksheet.Dimension?.Rows ?? 0;
                        if (rowCount < 2)
                        {
                            ErrorMessage = "The Excel file must contain headers and at least one data row.";
                            return Page();
                        }

                        var greetings = new List<GreetingData>();
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var propertyName = worksheet.Cells[row, 1]?.Value?.ToString()?.Trim();
                                var location = worksheet.Cells[row, 2]?.Value?.ToString()?.Trim();
                                var bookerName = worksheet.Cells[row, 3]?.Value?.ToString()?.Trim();
                                var arrivalStr = worksheet.Cells[row, 4]?.Value?.ToString()?.Trim();
                                var departureStr = worksheet.Cells[row, 5]?.Value?.ToString()?.Trim();
                                var reservationNo = worksheet.Cells[row, 6]?.Value?.ToString()?.Trim();
                                var phone = worksheet.Cells[row, 7]?.Value?.ToString()?.Trim();

                                if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(bookerName))
                                    continue;

                                if (DateTime.TryParse(arrivalStr, out var arrival) &&
                                    DateTime.TryParse(departureStr, out var departure))
                                {
                                    var greeting = new GreetingData
                                    {
                                        PropertyName = propertyName,
                                        Location = location,
                                        BookerName = bookerName,
                                        Arrival = arrival,
                                        Departure = departure,
                                        ReservationNo = reservationNo,
                                        Phone = phone,
                                        WhatsAppMessage = GenerateWhatsAppMessage(reservationNo, arrival, departure, location, bookerName)
                                    };

                                    greetings.Add(greeting);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log and skip invalid rows
                                continue;
                            }
                        }

                        if (greetings.Count == 0)
                        {
                            ErrorMessage = "No valid greeting data found in the Excel file.";
                            return Page();
                        }

                        Greetings = greetings;
                        SuccessMessage = $"Successfully loaded {greetings.Count} greeting(s).";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred while processing the file: {ex.Message}";
            }

            return Page();
        }

        private string GenerateWhatsAppMessage(string reservationNo, DateTime arrival, DateTime departure, string location, string bookerName)
        {
            // Extract first name from booker name
            string firstName = bookerName?.Split(' ')[0] ?? "Guest";

            string message = $"Booking.com Reservation No: {reservationNo} - ({arrival:dd MMM yyyy} - {departure:dd MMM yyyy}) - {location}" +
                Environment.NewLine + Environment.NewLine +
                $"Hi {firstName}," + Environment.NewLine +
                "I am Nima your host for booking in London." + Environment.NewLine +
                Environment.NewLine +
                "*Check-in at 3 PM*" + Environment.NewLine +
                "*Luggage drop off after 11 AM*" + Environment.NewLine +
                Environment.NewLine +
                "Check-in instruction will be sent to you tomorrow." + Environment.NewLine +
                "Could you please share your estimated arrival time?";

            return message;
        }
    }
}