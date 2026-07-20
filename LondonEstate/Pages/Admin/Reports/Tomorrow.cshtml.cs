using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;

namespace LondonEstate.Pages.Admin.Reports
{
    public class GreetingData
    {
        public string PropertyName { get; set; }
        public string Location { get; set; }
        public string ArrivalTime { get; set; }
        public string ReservationNo { get; set; }
        public string Phone { get; set; }
    }

    [Authorize]
    public class TomorrowModel : PageModel
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
                                var reservationNo = worksheet.Cells[row, 12]?.Value?.ToString()?.Trim();
                                var phone = worksheet.Cells[row, 13]?.Value?.ToString()?.Trim();
                                var arrivalStr = worksheet.Cells[row, 14]?.Value?.ToString()?.Trim();



                                var greeting = new GreetingData
                                {
                                    PropertyName = propertyName,
                                    Location = location,
                                    ArrivalTime = !string.IsNullOrEmpty(arrivalStr) ? DateTime.Parse(arrivalStr).ToString("HH:mm") : "N/A",
                                    ReservationNo = reservationNo,
                                    Phone = phone
                                };

                                greetings.Add(greeting);
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

                        Greetings = greetings.OrderBy(g => g.PropertyName).ToList();
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
    }
}