using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LondonEstate.Pages.Admin.Contract;

public class IndexModel : PageModel
{
    [BindProperty]
    public string SortCode { get; set; } = string.Empty;

    [BindProperty]
    public string Account { get; set; } = string.Empty;

    [BindProperty]
    public string Rent { get; set; } = string.Empty;

    [BindProperty]
    public string GuestName { get; set; } = string.Empty;

    public string? Message { get; set; }

    public void OnGet()
    {
        SortCode = "30-99-50";
        Account = "26105560";
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            Message = "Please fill in all required fields.";
            return Page();
        }

        try
        {
            // Generate the PDF with replaced placeholders
            var pdfBytes = GeneratePdf();

            // Return the PDF as a downloadable file
            return File(pdfBytes, "application/pdf", $"GuestAgreement_{DateTime.Now:yyyyMMdd}_{GuestName}.pdf");
        }
        catch (Exception ex)
        {
            Message = $"Error generating PDF: {ex.Message}";
            return Page();
        }
    }

    private byte[] GeneratePdf()
    {
        using var memoryStream = new MemoryStream();
        using var writer = new PdfWriter(memoryStream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf);

        // Set up fonts - Using Aptos font from files
        PdfFont regularFont;
        PdfFont boldFont;

        // Option 1: Load from Fonts folder in project
        var fontsPath = Path.Combine(Directory.GetCurrentDirectory(), "Fonts");
        var aptosRegularPath = Path.Combine(fontsPath, "aptos.ttf");
        var aptosBoldPath = Path.Combine(fontsPath, "aptos-bold.ttf");

        if (System.IO.File.Exists(aptosRegularPath) && System.IO.File.Exists(aptosBoldPath))
        {
            // Load Aptos fonts from files
            regularFont = PdfFontFactory.CreateFont(aptosRegularPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            boldFont = PdfFontFactory.CreateFont(aptosBoldPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
        }
        else
        {
            // Option 2: Try to load from system fonts
            try
            {
                regularFont = PdfFontFactory.CreateFont("Aptos", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                boldFont = PdfFontFactory.CreateFont("Aptos-Bold", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            }
            catch
            {
                // Option 3: Fallback to Calibri (similar modern font)
                try
                {
                    regularFont = PdfFontFactory.CreateFont("Calibri", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                    boldFont = PdfFontFactory.CreateFont("Calibri-Bold", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                }
                catch
                {
                    // Final fallback to standard fonts
                    regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                }
            }
        }

        // Calculate dates
        var today = DateTime.Now.ToString("dd/MM/yyyy");
        var tomorrow = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");

        // Replace placeholders with user input
        var template = GetTemplate();
        var content = template
            .Replace("#TODAY#", today)
            .Replace("#SORTCODE#", SortCode)
            .Replace("#ACCOUNT#", Account)
            .Replace("#RENT#", Rent)
            .Replace("#GUEST#", GuestName)
            .Replace("#TOMORROW#", tomorrow);

        // Split content into lines
        var lines = content.Split('\n');

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine))
            {
                // Add spacing for empty lines
                document.Add(new Paragraph("\u00A0").SetFontSize(8));
                continue;
            }

            var paragraph = new Paragraph(trimmedLine);

            // Apply styling based on content
            if (trimmedLine.StartsWith("Guest Agreement") ||
                trimmedLine.StartsWith("London Estate"))
            {
                paragraph.SetFont(boldFont)
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.CENTER);
            }
            else if (trimmedLine.Contains("#LINE#"))
            {
                AddHorizontalLine(document);
                continue; // Skip adding this line as text
            }
            else if (trimmedLine.Contains("1. House Rules") ||
                     trimmedLine.Contains("2. Security Deposit") ||
                     trimmedLine.Contains("3. Consequences of Violations") ||
                     trimmedLine.Contains("4. Acknowledgment and Agreement"))
            {
                paragraph.SetFont(boldFont)
                        .SetFontSize(12)
                        .SetMarginTop(10)
                        .SetMarginBottom(5);
            }
            else if (trimmedLine.StartsWith("No ") ||
                     trimmedLine.StartsWith("Registered ") ||
                     trimmedLine.StartsWith("Noise ") ||
                     trimmedLine.StartsWith("Smoke "))
            {
                paragraph.SetFont(boldFont)
                        .SetFontSize(10)
                        .SetMarginTop(8);
            }
            else if (trimmedLine.StartsWith("*"))
            {
                paragraph.SetMarginLeft(20)
                        .SetFontSize(10)
                        .SetFont(regularFont);
            }
            else if (trimmedLine.Contains("Signature:"))
            {
                paragraph.SetFontSize(10)
                        .SetMarginTop(10)
                        .SetFont(regularFont);
            }
            else if (trimmedLine.StartsWith("Sort Code:") ||
                     trimmedLine.StartsWith("Account Number:") ||
                     trimmedLine.StartsWith("Rent:") ||
                     trimmedLine.StartsWith("Guest Name:") ||
                     trimmedLine.StartsWith("Check-in Date:") ||
                     trimmedLine.StartsWith("Check-out Date:"))
            {
                paragraph.SetFont(boldFont)
                        .SetFontSize(11)
                        .SetMarginTop(3);
            }
            else
            {
                paragraph.SetFontSize(10)
                        .SetFont(regularFont);
            }

            document.Add(paragraph);
        }

        document.Close();
        return memoryStream.ToArray();
    }

    // Simple method to add horizontal line
    private void AddHorizontalLine(Document document)
    {
        var line = new LineSeparator(new SolidLine(1f));
        line.SetStrokeColor(ColorConstants.DARK_GRAY);
        line.SetMarginTop(10);
        line.SetMarginBottom(10);
        document.Add(line);
    }
    private string GetTemplate()
    {
        return @"Guest Agreement – #TODAY#
London Estate & Letting Agents Ltd
Sort Code: #SORTCODE#
Account Number: #ACCOUNT#
Rent: £#RENT#
Let me know once the payment has been made. Looking forward to welcoming you!
Guest Name: #GUEST#
Check-in Date: #TODAY# – 3:00 PM
Check-out Date: #TOMORROW# – 11:00 AM
#LINE#
1. House Rules
To ensure a pleasant stay for all guests and neighbours, please adhere to the following rules:
No Parties or Events: 
Parties, gatherings, or events are strictly prohibited. Any such activity will result in immediate eviction without a refund.
No Smoking: 
Smoking is not permitted inside the property. Evidence of smoking indoors will lead to forfeiture of the security deposit.
Registered Guests Only: 
Only the individuals listed in the booking are allowed to stay overnight. Unauthorized guests may result in immediate eviction and loss of the security deposit.
Noise Levels: 
Please keep noise to a minimum between 10:00 PM and 8:00 AM. The property is equipped with noise sensors to monitor sound levels.
Smoke Detectors: 
The property is equipped with smoke detectors for your safety. Tampering with these devices is prohibited and may result in penalties.
#LINE#
2. Security Deposit
A security deposit of £100 is required and will be held to cover any potential damages or violations of this agreement. The deposit will be refunded within 7 days after check-out, provided:
* No damage is done to the property or its contents.
* No violations of the house rules occur.
* All keys are returned, and the property is left in good condition.
#LINE#
3. Consequences of Violations
Failure to comply with the house rules may result in:
* Immediate eviction from the property without a refund.
* Forfeiture of the security deposit.
* Additional charges for damages or extra cleaning.
#LINE#
4. Acknowledgment and Agreement
By signing below, you acknowledge that you have read, understood, and agree to abide by the terms outlined in this Guest Agreement.
Guest Signature: ___________________________ 
Date: #TODAY#
Host Signature: Sina Haghighat parast 
Date: #TOMORROW#";
    }
}
